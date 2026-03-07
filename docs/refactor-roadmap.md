# OmadaPOS — Refactor Roadmap

> **Última revisión:** Marzo 2026  
> **Basado en:** Auditoría técnica completa post Fases 1–5  
> **Estado general:** Arquitectura desacoplada ✅ | Design System parcialmente adoptado ⚠️ | Deuda técnica media-baja 🟡

---

## Índice

- [PENDIENTE](#pendiente)
- [EN PROGRESO](#en-progreso)
- [COMPLETADO](#completado)
- [Resumen Ejecutivo](#resumen-ejecutivo)

---

## PENDIENTE

---

### 🔴 ALTA PRIORIDAD

---

**[ARCH-01] Restaurar funcionalidad de Gift Card**

Impacto: **ALTO**  
Archivos:
- `OmadaPOS/Views/frmGiftCard.cs` (líneas 37–64)

Descripción:  
El método `buttonPay_Click` tiene toda la lógica de procesamiento de pago comentada (40+ líneas). El formulario actualmente solo lee el saldo de la tarjeta pero **nunca lo descuenta ni confirma el pago**. La integración con `IHomeInteractionService` para notificar el resultado también está pendiente. Es un flujo de pago completamente no operativo.

Acción requerida:
1. Descomentar y refactorizar `buttonPay_Click` usando `_homeInteractionService.RequestGiftCardPaymentAsync(...)`.
2. Asegurarse que `frmGiftCard` recibe `IHomeInteractionService` vía constructor o servicio.
3. Agregar el handler correspondiente en `frmHome`.

---

**[ARCH-02] Cachear AdminSetting en PaymentCoordinatorService**

Impacto: **ALTO**  
Archivos:
- `OmadaPOS/Services/POS/PaymentCoordinatorService.cs` (líneas 69, 96, 115, 186, 210, 244)

Descripción:  
`_adminSettingService.LoadSettingById(WindowsIdProvider.GetMachineGuid())` se llama **6 veces** — una por cada método de pago, incluyendo `ProcessCashSaleAsync`, `ProcessGiftCardAsync`, `ProcessTerminalPaymentAsync`, `ProcessPaymentAsync`, `ProcessMultiplePaymentsAsync` y `PlaceOrderAsync`. Cada llamada realiza un hit a SQLite. En flujos de pago donde se encadenan varios métodos, esto provoca hasta 6 lecturas consecutivas de la misma fila de configuración.

Acción requerida:
1. Agregar campo privado: `private AdminSetting? _cachedConfig;`
2. Crear método helper `GetConfigAsync()` que use lazy-load con caché.
3. Reemplazar todas las llamadas directas por este helper.

---

**[DS-01] Agregar `readonly` a todos los campos de AppColors**

Impacto: **ALTO**  
Archivos:
- `OmadaPOS/Styles/AppColors.cs` (36 campos, todos sin `readonly`)

Descripción:  
Los 36 tokens de color son `public static Color` (mutables). Cualquier línea del código puede sobrescribir `AppColors.NavyDark = Color.Red` y corromper toda la paleta en runtime sin advertencia del compilador. El uso correcto es `public static readonly Color`.

Ejemplo del problema:
```csharp
// ACTUAL (mutable — peligroso):
public static Color NavyDark = Color.FromArgb(15, 23, 42);

// CORRECTO (inmutable):
public static readonly Color NavyDark = Color.FromArgb(15, 23, 42);
```

Acción requerida: Agregar `readonly` a los 36 campos de color (no afecta los métodos de gradiente).

---

**[DS-02] Externalizar el porcentaje de descuento del PricingEngine**

Impacto: **ALTO**  
Archivos:
- `OmadaPOS/Domain/Services/PricingEngine.cs` (línea 55)
- `OmadaPOS/Domain/Services/IPricingEngine.cs`

Descripción:  
El 30% de descuento está hardcodeado directamente en el motor de precios:
```csharp
decimal totalDiscount = applyDiscount ? (subTotal + totalTax) * 0.30m : 0m;
```
Esto viola el principio de responsabilidad única y hace que cambiar las reglas de descuento requiera modificar el dominio. El descuento debería venir de configuración o de una política externa.

Acción requerida:
1. Agregar `DiscountRate` a `AdminSetting` (o crear `DiscountPolicy` análogo a `SurchargePolicy`).
2. Modificar `IPricingEngine.ComputeCartTotals` para aceptar `decimal discountRate`.
3. El llamador (`PaymentCoordinatorService`) pasa el rate desde la configuración.

---

**[ARCH-03] Eliminar Service Locator en formularios de alta prioridad**

Impacto: **ALTO**  
Archivos:
- `OmadaPOS/Views/frmSignIn.cs` (líneas 20–21): `IUserService`, `IWindowService`
- `OmadaPOS/Views/frmSplit.cs` (líneas 55–62): 7 dependencias
- `OmadaPOS/Views/frmCustomerScreen.cs` (líneas 24–25): `IBannerService`, `IShoppingCart`

Descripción:  
`Program.GetService<T>()` en el constructor de un formulario es Service Locator — una inversión oculta de control que impide testear unitariamente las clases, oculta las dependencias reales de la clase y viola el principio de dependencias explícitas. `WindowService` ya usa `ActivatorUtilities.CreateInstance`, por lo que declarar parámetros en el constructor resuelve el problema sin cambios adicionales en el contenedor.

Acción requerida (por formulario):
1. Declarar dependencias como parámetros del constructor.
2. Eliminar las llamadas `Program.GetService<T>()` del cuerpo del constructor.
3. Asegurar que `WindowService.OpenX()` pasa los argumentos necesarios.

---

### 🟡 PRIORIDAD MEDIA

---

**[DS-03] Unificar el patrón de header en formularios popup**

Impacto: **MEDIO**  
Archivos:
- `OmadaPOS/Views/frmPopupQuantity.cs` (líneas 62–95)
- `OmadaPOS/Views/frmProductNew.cs` (líneas 55–97)
- `OmadaPOS/Views/frmKeyLookup.cs` (líneas 48–85)
- `OmadaPOS/Views/frmCheckPrice.cs` (líneas 35–92)

Descripción:  
Los 4 formularios popup comparten un patrón de header idéntico que está duplicado en cada archivo:
```csharp
// Duplicado en 4 formularios:
new Font("Montserrat", 22F, FontStyle.Bold)     // debería ser AppTypography.AmountDisplay
new Font("Montserrat", 16F, FontStyle.Bold)     // sin token en AppTypography
new Font("Segoe UI", 10F, FontStyle.Regular)    // debería ser AppTypography.BodySmall
new Padding(16, 0, 16, 0)                       // debería ser AppSpacing.HeaderTitle
Color.White                                      // debería ser AppColors.TextWhite
Color.FromArgb(200, 255, 255, 255)              // no existe en ningún token
```

Acción requerida:
1. Agregar token `AppTypography.PopupTitle = new Font("Montserrat", 16F, FontStyle.Bold)`.
2. Agregar token `AppColors.OverlayLight = Color.FromArgb(200, 255, 255, 255)` en `AppColors`.
3. Extraer método `EstiloFormularioPOS.BuildContextualHeader(string icon, string title)` que centralice el patrón.
4. Reemplazar los 4 bloques duplicados con la llamada al método centralizado.

---

**[ARCH-04] Eliminar Service Locator en formularios de prioridad media**

Impacto: **MEDIO**  
Archivos:
- `OmadaPOS/Views/frmHold.cs` (líneas 33–35)
- `OmadaPOS/Views/frmGiftCard.cs` (línea 29)
- `OmadaPOS/Views/frmCheckPrice.cs` (líneas 15–16)
- `OmadaPOS/Views/frmCierreDiario.cs` (líneas 16–17)
- `OmadaPOS/Views/frmSetting.cs` (línea 17)
- `OmadaPOS/Views/frmPopupCashPayment.cs` (líneas 27–28)
- `OmadaPOS/Views/frmPrintInvoice.cs` (líneas 17–18)
- `OmadaPOS/Views/frmPopupQuantity.cs` (línea 19)
- `OmadaPOS/Views/frmProductNew.cs` (línea 16)
- `OmadaPOS/Views/frmKeyLookup.cs` (línea 14)
- `OmadaPOS/Views/frmProductNoExist.cs` (línea 20)

Descripción:  
11 formularios adicionales usan `Program.GetService<T>()`. El patrón de solución es idéntico al descrito en ARCH-03. Se separan en este ítem por volumen de trabajo, no por diferencia técnica.

---

**[DS-04] Extraer configuración de ListView a ListViewThemer**

Impacto: **MEDIO**  
Archivos:
- `OmadaPOS/Views/frmCustomerScreen.cs` (líneas 115–200)
- `OmadaPOS/Views/frmPrintInvoice.cs` (líneas 60–110)
- `OmadaPOS/Presentation/Controls/CartListViewControl.cs` (líneas 43–94)

Descripción:  
La configuración de `ListView` (columnas, owner-draw de headers con fondo navy, filas zebra, ajuste de anchos) está duplicada en al menos 3 lugares con código prácticamente idéntico. `frmCustomerScreen` y `CartListViewControl` comparten el mismo paint handler para headers.

Acción requerida:
1. Crear `OmadaPOS/Presentation/Styling/ListViewThemer.cs`.
2. Métodos estáticos: `ApplyNavyHeader(ListView lv)`, `ApplyZebraRows(ListView lv)`, `AdjustColumnsEqually(ListView lv)`.
3. Reemplazar los 3 bloques duplicados con llamadas al themer.

---

**[COMP-01] Refactorizar AutocompleteProductUserControl**

Impacto: **MEDIO**  
Archivos:
- `OmadaPOS/Componentes/AutocompleteProductUserControl.cs` (línea 18)

Descripción:  
Un `UserControl` de presentación resuelve `ICategoryService` vía `Program.GetService<ICategoryService>()`. Los controles de UI no deberían tener conocimiento del contenedor DI. El control debería recibir datos procesados desde afuera o exponer un evento para que el formulario padre los cargue.

Acción requerida:
1. Exponer propiedad o método `LoadSuggestions(IEnumerable<string> products)`.
2. Eliminar la dependencia de `ICategoryService` del control.
3. El formulario que instancia el control se encarga de resolver el servicio y llamar al método.

---

**[DS-05] Agregar tokens faltantes en AppColors y AppTypography**

Impacto: **MEDIO**  
Archivos:
- `OmadaPOS/Styles/AppColors.cs`
- `OmadaPOS/Presentation/Styling/AppTypography.cs`

Descripción:  
Varios colores y fuentes se usan en el código pero no tienen token formal:

| Valor hardcodeado | Ubicación | Token propuesto |
|---|---|---|
| `Color.FromArgb(185, 28, 28)` | `AppColors.DangerGradient` (línea 83) | `AppColors.DangerDark` |
| `Color.FromArgb(180, 30, 30)` | `UserSessionControl.cs` línea 141 | `AppColors.DangerDeep` o usar `AppColors.Danger` |
| `Color.FromArgb(200, 255, 255, 255)` | 4 formularios popup | `AppColors.OverlayLight` |
| `Color.FromArgb(80, 0, 0, 0)` | `frmCierreDiario`, `frmCheckPrice` | `AppShadows.Overlay` |
| `new Font("Montserrat", 16F, FontStyle.Bold)` | 4 formularios popup | `AppTypography.PopupTitle` |
| `new Font("Segoe UI", 10F)` | `UserSessionControl` línea 234 | `AppTypography.ChevronIcon` |

---

**[DS-06] Alinear frmSignIn.Designer.cs con AppColors**

Impacto: **MEDIO**  
Archivos:
- `OmadaPOS/Views/frmSignIn.Designer.cs` (líneas 279, 283, 302, 326)

Descripción:  
El Designer de `frmSignIn` usa una paleta paralela informal:
- `Color.FromArgb(13, 31, 45)` → debería ser `AppColors.NavyDark` (15, 23, 42) — diferencia sutil pero diverge
- `Color.FromArgb(0, 166, 80)` → debería ser `AppColors.AccentGreen` (5, 150, 105)
- `Color.FromArgb(113, 128, 150)` → debería ser `AppColors.SlateBlue` (100, 116, 139)

La pantalla de login tiene su propia paleta "hardcodeada" en el Designer que no refleja `AppColors`.

---

**[DS-07] Aplicar tokens de AppSpacing en POSHeaderControl**

Impacto: **MEDIO**  
Archivos:
- `OmadaPOS/Presentation/Controls/POSHeaderControl.cs` (líneas 82–88)

Descripción:  
Los 8 anchos de columna del `TableLayoutPanel` del header están hardcodeados en píxeles absolutos (`170, 110, 110, 200, 130`). No corresponden a ningún token del Design System.

Acción requerida:
1. Agregar constantes a `AppSpacing`: `ColumnHeader = 170`, `ColumnAction = 110`, `ColumnSession = 200`, `ColumnClose = 130`.
2. Reemplazar los literales en `ApplyTheme()`.

---

### 🟢 PRIORIDAD BAJA

---

**[COMP-02] Reemplazar colores hardcodeados en AbecedarioControl**

Impacto: **BAJO**  
Archivos:
- `OmadaPOS/Componentes/AbecedarioControl.cs` (líneas 71–72)

Descripción:  
El botón "ALL" (Clear) usa colores hover/pressed que no vienen de `AppColors`:
```csharp
button.FlatAppearance.MouseOverBackColor = Color.FromArgb(170, 40, 40);
button.FlatAppearance.MouseDownBackColor = Color.FromArgb(140, 30, 30);
```
Deberían usar `ElegantButtonStyles.Darken(AppColors.Danger, 0.15f)` y `ElegantButtonStyles.Darken(AppColors.Danger, 0.25f)`.

---

**[COMP-03] Reemplazar font y color hardcodeados en KeyPadControl**

Impacto: **BAJO**  
Archivos:
- `OmadaPOS/Componentes/KeyPadControl.cs` (líneas 48, 51)

Descripción:
```csharp
// Línea 48: sin token
_display.Font = new Font("Consolas", 30F, FontStyle.Bold);
// Línea 51: sin token
_display.Padding = new Padding(0, 0, 12, 0);
```
Acción requerida: Agregar `AppTypography.KeypadDisplay = new Font("Consolas", 30F, FontStyle.Bold)` y una constante de padding en `AppSpacing`.

---

**[ARCH-05] Documentar la unidad de peso en CartItem y BarcodeSaleService**

Impacto: **BAJO**  
Archivos:
- `OmadaPOS/Models/CartItem.cs` (propiedad `Peso`)
- `OmadaPOS/Services/POS/BarcodeSaleService.cs` (línea que divide por 1000)

Descripción:  
`AddWeightedProductAsync` divide el peso por 1000 asumiendo que la báscula envía gramos. Si la báscula envía kg, este cálculo introduce un error de factor 1000. No hay documentación de la unidad de entrada.

Acción requerida: Agregar comentarios XML claros en `CartItem.Peso` y en `BarcodeSaleService` indicando la unidad de la báscula y la conversión aplicada.

---

**[ARCH-06] Eliminar el catch silencioso en ShoppingCart**

Impacto: **BAJO**  
Archivos:
- `OmadaPOS/Services/ShoppingCart.cs` (líneas ~115, ~160, ~190)

Descripción:  
Los métodos `AddItem`, `UpdateQuantity` y `RemoveItem` capturan `Exception` y retornan `false` silenciosamente. Un error de SQLite o de threading quedaría completamente oculto. El `LoadCartAsync` tiene un `catch { throw; }` redundante que no aporta nada.

Acción requerida:
1. En `AddItem`, `UpdateQuantity`, `RemoveItem`: loggear la excepción antes de retornar `false`.
2. En `LoadCartAsync`: eliminar el bloque `catch { throw; }` vacío o loguearlo.

---

## EN PROGRESO

---

**[PHASE-05-DS] Adopción del Design System — Formularios popup pendientes**

Impacto: **MEDIO**  
Archivos actualmente parciales:
- `OmadaPOS/Views/frmPopupQuantity.cs`
- `OmadaPOS/Views/frmProductNew.cs`
- `OmadaPOS/Views/frmKeyLookup.cs`
- `OmadaPOS/Views/frmCheckPrice.cs`
- `OmadaPOS/Views/frmCierreDiario.cs`

Estado:  
El Design System está definido completamente (Fase 5) y se adoptó en los `UserControls` del home y en los formularios principales. Los formularios popup listados arriba aún usan `new Font(...)`, `Color.White` y `Color.FromArgb(...)` directamente. La tarea está "en progreso" porque los tokens ya existen — solo falta aplicarlos.

---

**[PHASE-04-PRICING] PricingEngine — extensibilidad de promociones**

Impacto: **MEDIO**  
Archivos:
- `OmadaPOS/Domain/Services/PricingEngine.cs`
- `OmadaPOS/Domain/Services/IPricingEngine.cs`

Estado:  
El motor está centralizado y funciona correctamente para "Price" (N por $X) y peso. La extensibilidad para nuevos tipos de promoción aún no está diseñada (patrón Strategy pendiente). El `Discount_Amount` por línea en `CartModel` siempre es 0 aunque exista descuento global.

---

## COMPLETADO

---

**[✅ PHASE-01] Extracción de lógica de negocio de frmHome a servicios**

Archivos:
- `OmadaPOS/Services/POS/HomeInitializationService.cs`
- `OmadaPOS/Services/POS/BarcodeSaleService.cs`
- `OmadaPOS/Services/POS/PaymentCoordinatorService.cs`
- `OmadaPOS/Services/POS/HoldCartService.cs`
- `OmadaPOS/Services/POS/CashDrawerService.cs`

Resultado: `frmHome` delegó toda la lógica de dominio a 5 servicios especializados con interfaces propias. La forma actúa como coordinador de UI.

---

**[✅ PHASE-02] División de UI de frmHome en UserControls**

Archivos:
- `OmadaPOS/Presentation/Controls/POSHeaderControl.cs`
- `OmadaPOS/Presentation/Controls/ScanInputControl.cs`
- `OmadaPOS/Presentation/Controls/CartListViewControl.cs`
- `OmadaPOS/Presentation/Controls/CartTotalsControl.cs`
- `OmadaPOS/Presentation/Controls/PaymentPanelControl.cs`
- `OmadaPOS/Presentation/Controls/UserSessionControl.cs`

Resultado: 6 `UserControl` extraídos con patrón `static Attach()`. `frmHome` es un coordinador de menos de 500 líneas. Compatibilidad total con WinForms Designer preservada.

---

**[✅ PHASE-03] Eliminación de acoplamiento entre formularios**

Archivos:
- `OmadaPOS/Services/Navigation/IWindowService.cs`
- `OmadaPOS/Services/Navigation/WindowService.cs`
- `OmadaPOS/Services/Navigation/IHomeInteractionService.cs`
- `OmadaPOS/Services/Navigation/HomeInteractionService.cs`
- `OmadaPOS/Program.cs`

Resultado: **0 casts `((frmHome)Owner)`**. **0 instanciaciones `new frmX()`** fuera de `WindowService`. Todos los formularios se crean vía `ActivatorUtilities.CreateInstance`. `frmHome` no es referenciado directamente por ningún otro formulario.

---

**[✅ PHASE-04] Centralización de cálculos en PricingEngine**

Archivos:
- `OmadaPOS/Domain/Services/IPricingEngine.cs`
- `OmadaPOS/Domain/Services/PricingEngine.cs`
- `OmadaPOS/Models/CartItem.cs`
- `OmadaPOS/Services/ShoppingCart.cs`
- `OmadaPOS/Services/OrderApplicationService.cs`

Resultado: `CartItem` es entidad de datos pura. `PricingEngine` es la única fuente de verdad para subtotal, tax y total. `PromotionCalculator` y `CartCalculator` eliminados. `ShoppingCart` aplica pricing en cada mutación. Inconsistencia entre UI y API eliminada.

---

**[✅ PHASE-05] Implementación del Design System**

Archivos:
- `OmadaPOS/Presentation/Styling/AppTypography.cs` (20 tokens de fuente)
- `OmadaPOS/Presentation/Styling/AppSpacing.cs` (6 escalas + 10 presets)
- `OmadaPOS/Presentation/Styling/AppRadii.cs` (7 radios)
- `OmadaPOS/Presentation/Styling/AppBorders.cs` (5 colores + 3 anchos)
- `OmadaPOS/Presentation/Styling/AppShadows.cs` (5 niveles)
- `OmadaPOS/Presentation/Styling/ButtonVariants.cs` (14 variantes semánticas)
- `OmadaPOS/GlobalUsings.cs`

Resultado: Design System completo con `global using`. UserControls del home, `frmSplit`, `frmCustomerScreen` y `EstiloFormularioPOS` actualizados a tokens.

---

**[✅ DI-FRMSIGNIN] Constructor injection en frmHome**

Archivos:
- `OmadaPOS/Views/frmHome.cs`

Resultado: `frmHome` es el único formulario con **11 dependencias declaradas en constructor**, completamente libre de `Program.GetService`. Es el modelo de referencia para migrar los demás formularios.

---

**[✅ SURCHARGE] Política de recargo centralizada**

Archivos:
- `OmadaPOS/Domain/SurchargePolicy.cs`

Resultado: El recargo de 3.8% para crédito en la sucursal 45 está en una clase de dominio testeable, no inline en el formulario de pago.

---

**[✅ WEIGHT-BARCODE] Parser de códigos de barras por peso**

Archivos:
- `OmadaPOS/Domain/WeightBarcodeParser.cs`

Resultado: La lógica de parsing de barcodes GS1-128 con precio/peso embebido está en su propia clase de dominio.

---

**[✅ CART-THREAD-SAFETY] ShoppingCart thread-safe sin deadlock**

Archivos:
- `OmadaPOS/Services/ShoppingCart.cs`

Resultado: Operaciones en memoria dentro de `lock`. Persistencia SQLite en `Task.Run` fuera del lock. Elimina el riesgo de deadlock que existía con `GetAwaiter().GetResult()` dentro del lock.

---

## Resumen Ejecutivo

---

### Estado de Arquitectura

| Criterio | Estado | Notas |
|---|---|---|
| Separación UI / Lógica | ✅ Completo | 5 servicios POS + 6 UserControls |
| Eliminación de God Form | ✅ Completo | frmHome < 500 líneas |
| Desacoplamiento entre formularios | ✅ Completo | 0 casts, 0 `new frmX()` directos |
| Inyección de dependencias | ⚠️ Parcial | Solo frmHome usa constructor DI; 13 formularios usan Service Locator |
| Consistencia de lifetime (DI scope) | ⚠️ Revisar | `IShoppingCart` es Scoped pero se comporta como Singleton |
| Manejo de errores en servicios | ⚠️ Mejorable | Catch silencioso en ShoppingCart |

---

### Estado del Design System

| Criterio | Estado | Cobertura estimada |
|---|---|---|
| Tokens definidos | ✅ Completo | AppColors, Typography, Spacing, Radii, Borders, Shadows, ButtonVariants |
| Adoptado en UserControls del home | ✅ Completo | 6/6 controles usan tokens |
| Adoptado en formularios principales | ✅ Mayormente | frmSplit, frmCustomerScreen, frmHold |
| Adoptado en formularios popup | ⚠️ Pendiente | 4 popups con patrón hardcodeado duplicado |
| AppColors inmutables (`readonly`) | ❌ Faltante | 36 campos mutables — riesgo de runtime corruption |
| Tokens completos | ⚠️ Parcial | 6 colores/fuentes en uso sin token formal |

---

### Estado de Lógica de Negocio

| Criterio | Estado | Notas |
|---|---|---|
| PricingEngine centralizado | ✅ Completo | Subtotal, tax, total unificados |
| CartItem como entidad de datos | ✅ Completo | Sin propiedades computadas |
| Promoción tipo "Price" | ✅ Funciona | N items por $X correctamente implementado |
| Soporte de peso (Peso) | ✅ Funciona | PricingEngine prioriza Peso cuando > 0 |
| Descuento configurable | ❌ Faltante | 0.30m hardcodeado en PricingEngine |
| Extensibilidad de promociones | ⚠️ Limitado | Solo tipo "Price" implementado; sin Strategy |
| Gift Card operativa | ❌ Roto | Flujo de pago completamente comentado |
| Config de terminal cacheada | ❌ Faltante | 6 lecturas SQLite por operación de pago |

---

### Deuda Técnica Restante

| Categoría | Ítems pendientes | Esfuerzo estimado |
|---|---|---|
| Funcionalidades rotas | 1 (frmGiftCard) | 2–4 horas |
| Seguridad de datos (readonly) | 1 (AppColors) | 15 minutos |
| Performance | 1 (AdminSetting cache) | 1 hora |
| Service Locator → DI | 13 formularios | 3–6 horas |
| Design System incompleto | 4 popups + tokens faltantes | 2–3 horas |
| Duplicación de código | ListView (3 lugares) + headers (4 popups) | 2 horas |
| Extensibilidad de dominio | Descuento + Promociones | 3–5 horas |
| Calidad de código menor | AbecedarioControl, KeyPadControl, catches | 1 hora |

**Total estimado de deuda técnica restante: 15–25 horas de desarrollo**

---

### Top 5 Tareas Más Importantes

| # | ID | Tarea | Por qué es prioritaria |
|---|---|---|---|
| 1 | ARCH-01 | Restaurar funcionalidad de Gift Card | Flujo de pago completamente roto — impacto directo en operaciones |
| 2 | DS-01 | Agregar `readonly` a AppColors | Riesgo de corrupción de paleta en runtime — 15 minutos de trabajo |
| 3 | ARCH-02 | Cachear AdminSetting en PaymentCoordinator | 6 hits a SQLite por cada pago — el bottleneck más fácil de eliminar |
| 4 | DS-02 | Externalizar descuento del PricingEngine | Un valor hardcodeado en el motor de precios es un bug latente inevitable |
| 5 | ARCH-03 | Migrar frmSignIn, frmSplit, frmCustomerScreen a DI | Los 3 formularios más críticos del sistema aún usan Service Locator |

---

*Documento generado automáticamente desde auditoría técnica. Actualizar estado de ítems al completar cada tarea.*
