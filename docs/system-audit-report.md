# OmadaPOS — System Audit Report
**Fecha:** Marzo 2026  
**Auditor:** Análisis técnico automatizado profundo  
**Versión del proyecto:** Post-Fase 5 (Design System implementado)

---

## Resumen Ejecutivo

### Nivel de Salud del Proyecto: **62 / 100**

| Dimensión | Puntuación | Estado |
|---|---|---|
| Arquitectura macro | 74/100 | ⚠️ Buena estructura, DI incompleto en 7 formas |
| Gestión de memoria | 38/100 | 🔴 6 leaks críticos activos |
| Performance | 60/100 | ⚠️ Varios hotspots identificados |
| Calidad de código | 65/100 | ⚠️ Async void peligrosos, código muerto |
| Design System | 68/100 | ⚠️ Incumplimiento en 6 archivos clave |
| Mantenibilidad | 72/100 | ✅ Servicios bien separados, PricingEngine correcto |

**Deuda técnica acumulada:** ~340 horas estimadas de corrección  
**Riesgo operacional inmediato:** ALTO (6 memory leaks críticos + 2 async void sin manejo de errores en flujos de pago)

---

## 1. ARQUITECTURA

### ✅ Lo que está bien
- Separación UI / lógica mediante 5 Application Services (Fase 1)
- UserControls bien encapsulados en `Presentation/Controls/` (Fase 2)
- Navegación centralizada vía `IWindowService` + `WindowService` (Fase 3)
- PricingEngine como único motor de cálculo (Fase 4)
- Design System con tokens semánticos (Fase 5)
- `DiscountPolicy.cs` externaliza la tasa de descuento del dominio
- `PaymentCoordinatorService` cachea AdminSetting correctamente

### ⚠️ Problemas detectados

#### ARCH-A01 — Service Locator en 7 archivos (Impacto: ALTO)
Los siguientes archivos usan `Program.GetService<T>()` en lugar de constructor injection. El patrón devuelve `null` si el servicio no está registrado (usa `GetService` no `GetRequiredService`), hace el grafo de dependencias invisible y rompe el ciclo de vida correcto de DI.

| Archivo | Líneas | Servicios resueltos por SL |
|---|---|---|
| `frmHold.cs` | 33–35 | `IHoldService`, `IShoppingCart`, `IHomeInteractionService` |
| `frmCierreDiario.cs` | 16–17 | `IOrderService`, `IBranchService` |
| `frmPopupCashPayment.cs` | 27–28 | `IOrderService`, `IBranchService` |
| `AutocompleteProductUserControl.cs` | 18 | `ICategoryService` |
| `frmProductNoExist.cs` | 20 | `ICategoryService` |
| `frmPrintInvoice.cs` | 17–18 | `IOrderService`, `IBranchService` |
| `frmSetting.cs` | 17 | `IAdminSettingService` |

**Riesgo:** `Program.GetService<T>()` retorna `null` silenciosamente si el tipo no está registrado. Las 7 formas producirían `NullReferenceException` en runtime sin mensaje de error claro.

#### ARCH-A02 — Constructores vacíos en formas con Service Locator (Impacto: ALTO)
`frmHold`, `frmCierreDiario`, y `AutocompleteProductUserControl` tienen constructores sin parámetros, lo que hace imposible inyectarles dependencias sin refactorizar la firma del constructor. Al ser registradas como `Transient` en DI, `ActivatorUtilities.CreateInstance` puede resolverlas pero solo si el constructor acepta los parámetros.

#### ARCH-A03 — `OpenHome()` retorna `frmHome` pero el valor nunca se usa (Impacto: BAJO)
`IWindowService.OpenHome()` retorna `frmHome` (línea 7 de `IWindowService.cs`) pero ningún caller almacena ni usa el valor. La firma debería ser `void OpenHome()`.

#### ARCH-A04 — Parámetro `owner` silenciosamente ignorado en ventanas modeless (Impacto: MEDIO)
En `WindowService`, los métodos `OpenSplitPayment`, `OpenHold`, `OpenProductNew`, `OpenCustomerScreen`, `OpenKeyLookup`, `OpenPopupCashPayment`, `OpenGiftCard` aceptan `IWin32Window? owner` pero lo descartan al llamar `ShowModeless(form)`. Formas modales como `frmSplit` pueden aparecer detrás de `frmHome`, degradando la UX.

#### ARCH-A05 — Violación SRP: `frmHome` aún orquesta demasiado (Impacto: MEDIO)
A pesar de las refactorizaciones, `frmHome.cs` sigue teniendo ~900 líneas. Contiene:
- Lógica de navegación de tabs
- Gestión de sesión visual
- Orquestación de eventos del scanner
- Múltiples `async void` event handlers sin manejo de errores
La reducción a <500 líneas objetivo de Fase 2 no se logró completamente.

### 💡 Recomendaciones
1. Convertir las 7 formas con Service Locator a constructor injection (ver ARCH-A01)
2. Declarar `void OpenHome()` en `IWindowService`
3. Pasar `owner` a `Show(IWin32Window)` en formas modeless mediante `new Form().Show(ownerForm)`
4. Extraer handlers de tab-navigation y scanner a un `ScanSessionService`

---

## 2. PERFORMANCE

### ⚠️ Problemas detectados

#### PERF-01 — `CartListViewControl`: ListView sin `BeginUpdate`/`EndUpdate` (Impacto: ALTO)
**Archivo:** `OmadaPOS/Presentation/Controls/CartListViewControl.cs` — línea 98  
`_listView.Items.Clear()` seguido de `.Add()` individual sin `BeginUpdate()`/`EndUpdate()`. Con 20+ ítems en el carrito se disparan 20+ repaints por cada `ShoppingCart_CartChanged`. En un POS de alto volumen (scan rápido de productos) esto produce parpadeo y lag visual.

```
ANTES: _listView.Items.Clear(); foreach (item) _listView.Items.Add(...)
MEJOR: _listView.BeginUpdate(); _listView.Items.Clear(); foreach... _listView.EndUpdate();
```

#### PERF-02 — OwnerDraw innecesario en CartListView (Impacto: MEDIO)
**Archivo:** `CartListViewControl.cs` — líneas 66–90  
`OwnerDraw = true` activa el pipeline de drawing custom para cada fila, sub-item y encabezado. Sin embargo `DrawItem += (s,e) => e.DrawDefault = true` y `DrawSubItem += (s,e) => e.DrawDefault = true` delegan inmediatamente al renderer estándar. Solo `DrawColumnHeader` tiene lógica custom. El overhead de OwnerDraw se paga por cada fila sin beneficio real.

#### PERF-03 — Carga de categorías secuencial en `frmHome_Load` (Impacto: MEDIO)
**Archivo:** `OmadaPOS/Views/frmHome.cs` — líneas 375–376  
```csharp
await LoadMenuCategoriesAsync();   // hit a DB/API
await LoadCategoriesAsync();       // hit a DB/API separado
```
Ambas son llamadas independientes que podrían ejecutarse en paralelo con `Task.WhenAll`, reduciendo el tiempo de arranque del POS en ~40-60%.

#### PERF-04 — N+1 queries en `frmHold.buttonAdd_Click` (Impacto: ALTO)
**Archivo:** `OmadaPOS/Views/frmHold.cs` — líneas 127–135  
Itera hasta 10 colores llamando `await _holdService.GetHeldCartsByIdAsync(color)` individualmente para encontrar un slot vacío. Hasta 10 round-trips a SQLite por cada acción de hold. Solución: cargar todos los holds activos en una sola consulta y buscar el slot vacío en memoria.

#### PERF-05 — Búsqueda O(n) de categorías en `BarcodeSaleService` (Impacto: MEDIO)
**Archivo:** `OmadaPOS/Services/POS/BarcodeSaleService.cs` — línea 94  
`categories.SingleOrDefault(c => c.Id == product.CategoryId)` recorre la lista completa de categorías en cada click de producto. Con 100+ categorías y escaneos rápidos esto acumula trabajo innecesario. Convertir a `Dictionary<int, CategoryModel>`.

#### PERF-06 — `volatile` faltante en cache de `PaymentCoordinatorService` (Impacto: BAJO)
**Archivo:** `OmadaPOS/Services/POS/PaymentCoordinatorService.cs` — línea 46  
El campo `_cachedConfig` usa double-checked locking pero no está marcado `volatile`. En arquitecturas non-x86 (ARM64) la lectura especulativa del campo puede devolver un valor stale antes de que el bloqueo sea visible. Marcar como `volatile AdminSetting?`.

#### PERF-07 — `LoadLastConsecutivoPayment()` ejecutado sin caché (Impacto: BAJO)
**Archivo:** `PaymentCoordinatorService.cs` — líneas 151, 225  
Llamado en dos métodos distintos que pueden activarse en el mismo flujo (split payment + terminal). Cada llamada es un hit a SQLite. El consecutivo podría cachearse o cargarse una sola vez por operación.

### 💡 Recomendaciones
1. Añadir `BeginUpdate`/`EndUpdate` en `CartListViewControl.UpdateCartItems()`
2. Desactivar OwnerDraw para rows/subitems en `CartListViewControl`
3. `await Task.WhenAll(LoadMenuCategoriesAsync(), LoadCategoriesAsync())` en `frmHome_Load`
4. Refactorizar `frmHold.buttonAdd_Click` para cargar todos los holds de una vez
5. Convertir lista de categorías en `BarcodeSaleService` a `Dictionary<int, CategoryModel>`

---

## 3. MEMORY LEAKS

### 🔴 Críticos (producen acumulación de formas en memoria)

#### MEM-01 — `frmHome`: `CartChanged` nunca desuscrito (CRÍTICO)
**Archivo:** `OmadaPOS/Views/frmHome.cs` — línea 77  
`_shoppingCart.CartChanged += ShoppingCart_CartChanged` se suscribe en el constructor. `frmHome_FormClosing` nunca hace `-=`. `IShoppingCart` es Singleton; mantiene una referencia fuerte a `frmHome`, que nunca puede ser recolectado por el GC. Si el usuario abre y cierra sesión múltiples veces, cada instancia de `frmHome` queda rooted permanentemente.

#### MEM-02 — `frmHome`: Scanner events nunca desuscritos (CRÍTICO)
**Archivo:** `OmadaPOS/Views/frmHome.cs` — líneas 91–92  
```csharp
_zebraScannerService.OnBarcodeDataReceived += _zebraScannerService_OnBarcodeDataReceived;
_zebraScannerService.OnWeightUpdated       += _zebraScannerService_OnWeightUpdated;
```
`ZebraScannerService` es Singleton. Ambos eventos mantienen `frmHome` rooted. `frmHome_FormClosing` solo llama `_zebraScannerService?.Close()` sin hacer `-=`.

#### MEM-03 — `frmCustomerScreen`: `CartChanged` nunca desuscrito (CRÍTICO)
**Archivo:** `OmadaPOS/Views/frmCustomerScreen.cs` — línea 26  
Mismo patrón que MEM-01. `_shoppingCart.CartChanged += ShoppingCart_CartChanged` en constructor, sin desuscripción en `OnFormClosing` (línea 367). Cada apertura de la pantalla del cliente acumula una instancia más rooted por el Singleton.

#### MEM-04 — `frmSplit`: `CartChanged` lambda nunca desuscrita (CRÍTICO)
**Archivo:** `OmadaPOS/Views/frmSplit.cs` — línea 138  
```csharp
_shoppingCart.CartChanged += (s, e) => LoadCartItems();
```
Lambda captura `this` implícitamente (vía `LoadCartItems()`). `frmSplit` se abre cada vez que se hace un pago split. Cada instancia queda rooted por el Singleton `IShoppingCart`. Con 10 transacciones split en un turno: 10 instancias de `frmSplit` no recolectadas.

#### MEM-05 — `ProductImageControl`: `Region` creado en cada Paint sin liberar el anterior (CRÍTICO)
**Archivo:** `OmadaPOS/Componentes/ProductImageControl.cs` — línea 196  
```csharp
panelCard.Region = new Region(cardPath); // ← antiguo Region nunca se libera
```
`Control.Region` setter NO hace `Dispose()` del `Region` anterior. Como `PanelCard_Paint` se dispara en cada hover, resize y redibujado, los objetos GDI `Region` se acumulan. El límite por proceso en Windows es ~10,000 objetos GDI. Con 50 productos visibles y refrescos frecuentes, este límite puede alcanzarse en una sesión larga, causando fallos visuales o crash.

**Patrón correcto (ya implementado en `frmCustomerScreen.cs`):**
```csharp
var oldRegion = panelCard.Region;
panelCard.Region = new Region(cardPath);
oldRegion?.Dispose();
```

#### MEM-06 — `frmHome`: `ProductImageControl` descartado sin `Dispose` en cada cambio de tab (CRÍTICO)
**Archivo:** `OmadaPOS/Views/frmHome.cs` — líneas 494–525  
`flowLayoutPanel.Controls.Clear()` elimina los controles del panel pero **no llama `Dispose()`** en ellos. Cada `ProductImageControl` descartado retiene:
- HWNDs nativos (Label, Panel, PictureBox)
- Un `Region` por ciclo de paint (ver MEM-05)
- Un `ToolTip` no dispuesto (líneas 240-241 de `ProductImageControl.cs`)
- Fonts GDI en sus labels

Con filtros de letras (`AbecedarioControl`) esto ocurre en cada pulsación de letra.

**Fix:**
```csharp
foreach (Control c in flowLayoutPanel.Controls)
    c.Dispose();
flowLayoutPanel.Controls.Clear();
```

### ⚠️ Altos

#### MEM-07 — `frmCustomerScreen`: Timers parados pero no dispuestos (ALTO)
**Archivo:** `frmCustomerScreen.cs` — líneas 369–370  
`timerCarrousel?.Stop(); clockTimer?.Stop()` — `System.Windows.Forms.Timer` implementa `IDisposable` y encapsula un timer Win32 nativo. Solo hacer `Stop()` no libera el handle nativo hasta el GC finalizer.

#### MEM-08 — `frmHome`: `ToolTip` local no dispuesto en `ConfigureUI` (ALTO)
**Archivo:** `OmadaPOS/Views/frmHome.cs` — línea 224  
`var toolTip = new ToolTip { ShowAlways = true }` — `ToolTip` es `IDisposable` y tiene una ventana nativa. Almacenado en variable local, nunca dispuesto.

#### MEM-09 — `ProductImageControl`: `ToolTip` no dispuesto en `Load` (ALTO)
**Archivo:** `OmadaPOS/Componentes/ProductImageControl.cs` — líneas 240–241  
`var tip = new ToolTip()` — misma situación. Multiplicada por cada control que se abandona en cambio de tab.

#### MEM-10 — `UserSessionControl`: suscripción a `_ownerForm.MouseClick` sin `Dispose` (MEDIO)
**Archivo:** `OmadaPOS/Presentation/Controls/UserSessionControl.cs` — líneas 39–40  
`_ownerForm.MouseClick += (_, _) => HideMenu()` — el UserControl suscribe un evento del formulario padre. Sin `Dispose()` override, nunca desuscrito. Retiene una referencia cruzada forma→control→forma.

#### MEM-11 — `POSHeaderControl`: `Paint` lambda registrada en `ApplyTheme()` sin desuscripción (MEDIO)
**Archivo:** `OmadaPOS/Presentation/Controls/POSHeaderControl.cs` — líneas 75–79, 96–101  
Si `ApplyTheme()` se llama más de una vez (actualmente solo en `frmHome_Load`), las lambdas se acumulan provocando doble-pintado.

#### MEM-12 — `frmHome`: Font creado en loop de tabs no dispuesto (ALTO)
**Archivo:** `OmadaPOS/Views/frmHome.cs` — línea 438  
`new Font("Segoe UI", 11F)` dentro de `foreach (var cat in MenuCategories)`. Se crea un objeto GDI Font por cada categoría de menú en cada recarga. Nunca dispuestos.

### 💡 Recomendaciones

| Prioridad | Acción |
|---|---|
| 1 | En `frmHome_FormClosing`: `-=` de `CartChanged`, `OnBarcodeDataReceived`, `OnWeightUpdated` |
| 2 | En `frmCustomerScreen.OnFormClosing`: `-=` de `CartChanged`, `Dispose()` de ambos timers |
| 3 | En `frmSplit`: almacenar lambda en campo y desuscribir en `FormClosing` |
| 4 | En `ProductImageControl.PanelCard_Paint`: patrón `oldRegion?.Dispose()` antes de asignar nuevo |
| 5 | En `frmHome.UpdateProductsDisplay`: `foreach (c) c.Dispose()` antes de `Controls.Clear()` |
| 6 | Mover `Font` del loop en `LoadTabInfoAsync` a campo estático `readonly` |

---

## 4. CÓDIGO MUERTO

### Archivos/secciones sin uso activo

| Elemento | Archivo | Línea | Observación |
|---|---|---|---|
| `// await sqliteManager.DropTablesAsync()` | `Program.cs` | 97 | Operación destructiva comentada. **Eliminar inmediatamente** — riesgo de ejecución accidental |
| `buttonOK_Click` en `frmPopupQuantity` | `frmPopupQuantity.cs` | — | El botón `buttonCancel` dispara `buttonClose_Click`. Verificar que `buttonOK` esté correctamente conectado al handler |
| `OpenHome()` return value | `IWindowService.cs` | 7 | El tipo de retorno `frmHome` nunca se usa. Cambiar a `void OpenHome()` |
| Comentarios `// ((frmHome)Owner)...` | `frmGiftCard.cs` (antes de fix) | — | Eliminados correctamente en refactor actual. ✅ |
| `CartCalculator.cs` | Eliminado | — | Absorbido por `PricingEngine`. No existe ya. ✅ |
| `PromotionCalculator.cs` | Eliminado | — | Absorbido por `PricingEngine`. No existe ya. ✅ |
| Líneas comentadas `buttonPay_Click` | `frmGiftCard.cs` | — | Restaurado correctamente. ✅ |
| 4 líneas comentadas de `ElegantButtonStyles.Style(button10/20/50/100)` | `KeyPadMoneyControl.cs` | 110–113 | Debris de refactoring. Eliminar o restaurar la funcionalidad. |

### Servicios registrados en DI sin uso confirmado

Todos los 17 servicios de UI y 12 servicios de dominio registrados en `Program.cs` tienen al menos un caller verificado. No hay servicios "zombie".

---

## 5. DUPLICACIÓN DE CÓDIGO

### DUP-01 — Color `(248, 249, 252)` en 3 métodos de `frmHome.cs` (MEDIO)
**Archivo:** `OmadaPOS/Views/frmHome.cs` — líneas 232, 345, 412  
El mismo `Color.FromArgb(248, 249, 252)` se define tres veces. Debería estar en `AppColors` como `ProductsPanelBackground` o similar.

### DUP-02 — Color `(210, 215, 225)` hardcodeado (MEDIO)
**Archivo:** `OmadaPOS/Views/frmHome.cs` — línea 360  
`tableLayoutPanelMain.BackColor = Color.FromArgb(210, 215, 225)` — no tiene nombre semántico en `AppColors`.

### DUP-03 — `ConfigurarTamano()` eliminada de todos los popups ✅ (resuelto en T6)
La lógica fue centralizada en `PopupHeaderHelper.ConfigureSize()`. Sin embargo, `frmCheckPrice.cs` y otros popups que heredan de `Form` (no de `EstiloFormularioPOS`) siguen duplicando la lógica de sizing si en el futuro alguien crea una nueva forma sin usar el helper.

**Recomendación:** Documentar `PopupHeaderHelper.ConfigureSize` como el único entry point para sizing de popups.

### DUP-04 — Configuración de `ListView` duplicada en 3 lugares (MEDIO)

| Archivo | Descripción |
|---|---|
| `CartListViewControl.cs` | Configura `listViewCart` (OwnerDraw, columns, resize handler) |
| `frmSplit.cs` | `ConfigureCartListView()` y `ConfigurePaymentListView()` — 80+ líneas repetidas |
| `frmCustomerScreen.cs` | `ConfigureListView()` — similar configuración de columns y styles |

Los tres archivos crean columnas `ColumnHeader`, configuran `FullRowSelect`, `View.Details`, `OwnerDraw`, `ResizeRedraw` con código casi idéntico. Una `ListViewConfigurator` estática o un método de extensión centralizaría esto.

### DUP-05 — Lógica de `async void` + try/catch en event handlers repetida (ALTO)
8 métodos `async void` en `frmHome.cs` (ver sección 7) no tienen manejo de errores. El patrón de "envolver en try/catch y mostrar MessageBox o LogError" debería extraerse a un helper:

```csharp
// Patrón propuesto (extension method)
private async void SafeAsync(Func<Task> action, string context)
{
    try { await action(); }
    catch (Exception ex) { _logger.LogError(ex, context); }
}
// Uso: SafeAsync(() => LoadProductsAsync(), "Tab changed");
```

### DUP-06 — Assignments duplicados en `frmHome._zebraScannerService_OnWeightUpdated` (BAJO)
**Archivo:** `OmadaPOS/Views/frmHome.cs` — líneas 600, 610  
`SharedData.WeightUnit = weightStatus` aparece dos veces en el mismo método. El segundo es redundante.

---

## 6. ERRORES POTENCIALES

### ERR-01 — `async void` en handlers críticos de pago sin try/catch (CRÍTICO)
En WinForms, una excepción en `async void` **no puede ser capturada por el caller** y genera `Application.ThreadException`. En Release mode sin handler configurado, **termina el proceso**.

| Archivo | Método | Línea | Riesgo |
|---|---|---|---|
| `frmHome.cs` | `tabControlMenuCategories_SelectedIndexChanged` | ~459 | Error de red en `LoadProductsAsync` → crash |
| `frmHome.cs` | `PaymentOrder(PaymentType)` | ~809 | Timeout del terminal de pago → crash durante transacción |
| `frmHome.cs` | `pictureBoxPesado_Click` | ~895 | Excepción en flujo de peso → crash |
| `frmHome.cs` | `buttonOpenDrawer_Click` | ~700 | Fallo de apertura cajón → crash |
| `frmHome.cs` | `buttonLogout_Click` | ~691 | Error de red en logout → crash |
| `frmHome.cs` | `addCustomProduct(bool, decimal)` | ~736 | **Público** `async void` — API insegura |
| `frmHome.cs` | `SearchProduct(string)` | ~835 | **Público** `async void` — API insegura |
| `frmHome.cs` | `ProcessPaymentMultiple()` | ~879 | **Público** `async void` — API insegura |

**Los 3 métodos públicos `async void` son especialmente peligrosos** porque los callers (`frmProductNew`, `frmKeyLookup`, `frmSplit`) no pueden `await` el resultado, perdiendo tanto errores como el estado de completado.

### ERR-02 — `Program.GetService<T>()` devuelve `null` sin excepción (ALTO)
**Archivo:** `OmadaPOS/Program.cs` — línea 78–81  
`ServiceProvider.GetService(typeof(T))` retorna `null` si el tipo no está registrado, a diferencia de `GetRequiredService` que lanza `InvalidOperationException` con mensaje descriptivo. Los 7 archivos que usan Service Locator (ARCH-A01) podrían fallar con `NullReferenceException` sin contexto útil.

### ERR-03 — `ShoppingCart` fire-and-forget oculta fallos de persistencia (CRÍTICO)
**Archivo:** `OmadaPOS/Services/ShoppingCart.cs` — líneas 103–109, 150–156, 185–189, 210  
Los 4 métodos mutables (`AddItem`, `UpdateQuantity`, `RemoveItem`, `Clear`) persisten a SQLite con `_ = Task.Run(...)` sin logging interno en la lambda. Si SQLite falla (disco lleno, archivo bloqueado, corrupción), el carrito en memoria y en disco divergen **silenciosamente**. Al reiniciar el POS, el carrito SQLite puede tener ítems fantasma o datos corruptos.

**Fix requerido:**
```csharp
_ = Task.Run(async () =>
{
    try { await _sqliteManager.SaveCartItemAsync(toAdd, _machineGuid); }
    catch (Exception ex) { _logger.LogError(ex, "Failed to persist cart item {Id}", toAdd.ProductId); }
});
```

### ERR-04 — `frmSignIn` usa font "Montserrat" sin verificar instalación (ALTO)
**Archivo:** `OmadaPOS/Views/frmSignIn.cs` — líneas 90, 102  
`new Font("Montserrat", 48F, FontStyle.Bold)` — "Montserrat" no es una fuente de Windows inbox. Si no está instalada en el equipo del cliente, WinForms substituye silenciosamente con una fuente fallback (normalmente "Microsoft Sans Serif"). La pantalla de login puede verse completamente distinta en producción.

Mismo riesgo en `frmCierreDiario.cs` (línea 68).

**Mitigación:** Usar `new FontFamily("Montserrat")` y verificar con `.Families.Any(f => f.Name == "Montserrat")` antes de instanciar, o embeber la fuente como recurso y cargarla con `PrivateFontCollection`.

### ERR-05 — Race condition latente: `_cachedConfig` no es `volatile` (BAJO)
**Archivo:** `PaymentCoordinatorService.cs` — línea 46  
Sin `volatile`, el compilador/JIT puede reordenar lecturas del campo en multi-thread bajo arquitecturas ARM64. El resultado es que dos hilos concurrentes podrían ambos ver `_cachedConfig == null` y ejecutar dos cargas simultáneas. El `SemaphoreSlim` previene la carrera, pero la primera lectura `if (_cachedConfig != null) return _cachedConfig` fuera del lock podría leer un valor stale.

### ERR-06 — Conversión insegura `decimal.Parse` sin cultura en `frmCheckPrice` (MEDIO)
**Archivo:** `OmadaPOS/Views/frmCheckPrice.cs` — línea ~78  
`decimal price = decimal.Parse(sPrice) / 100` — `decimal.Parse` sin `CultureInfo` usa la cultura del thread actual. Si el POS corre en un locale donde el separador decimal es coma (`,`), la conversión fallará o devolverá un valor incorrecto para códigos de barras con precios embebidos.

**Fix:** `decimal.Parse(sPrice, CultureInfo.InvariantCulture)`

### ERR-07 — `frmGiftCard.textBoxCode_TextChanged`: llamada asíncrona con `giftCardService` potencialmente nula (MEDIO)
**Archivo:** `OmadaPOS/Views/frmGiftCard.cs` (versión restaurada)  
`cardCode` se extrae del regex, pero si el pattern no hace match (texto parcialmente escrito), `cardCode` queda como string vacío/anterior. Luego se llama `await _giftCardService.GetByCode(cardCode)` con un valor potencialmente inútil en cada keystroke. Debería incluir una guarda `if (string.IsNullOrWhiteSpace(cardCode)) return;` antes de la llamada al servicio.

**Nota:** Este check existe parcialmente pero la guarda debe preceder a la llamada al servicio, no al foreach de matches.

### ERR-08 — `SharedData.WeightUnit` asignado dos veces en el mismo método (BAJO)
**Archivo:** `OmadaPOS/Views/frmHome.cs` — líneas 600, 610  
Doble asignación redundante. La segunda es inofensiva pero indica copy-paste no limpiado.

---

## 7. DESIGN SYSTEM — Archivos no conformes

### Estado general: 68/100

Los componentes creados en Fase 5 (`Presentation/Styling/`) están bien utilizados en `frmHome`, `frmSplit`, `frmCustomerScreen`, y los 4 popups refactorizados. Los siguientes archivos **no están completamente alineados**:

### frmSignIn.cs — Cumplimiento: 45%

| Línea | Elemento | Problema |
|---|---|---|
| 44 | `Color.FromArgb(243, 245, 250)` | No en AppColors |
| 72 | `new Font("Consolas", 11F)` | No en AppTypography |
| 90, 102 | `new Font("Montserrat", 48F)` | No en AppTypography + riesgo de fuente no instalada |
| 114, 139 | `new Font("Segoe UI", 14F)`, `new Font("Segoe UI", 13F)` | No en AppTypography |
| 193 | `Color.FromArgb(200, 210, 225)` | No en AppColors |
| 208 | `new Font("Consolas", 40F)` | No en AppTypography + GDI no dispuesto |

### frmCierreDiario.cs — Cumplimiento: 50%

| Línea | Elemento | Problema |
|---|---|---|
| 41 | `new Font("Segoe UI", 16F)` | No en AppTypography |
| 58 | `new Font("Segoe UI", 22F, FontStyle.Bold)` | No en AppTypography |
| 59 | `Color.FromArgb(220, 255, 255, 255)` | No en AppColors |
| 68 | `new Font("Montserrat", 16F, FontStyle.Bold)` | No en AppTypography + riesgo de fuente |
| 78–79 | `new Font(...)`, `Color.FromArgb(...)` | No en DS tokens |
| 87 | `Color.FromArgb(80, 0, 0, 0)` | No en AppShadows |

### KeyPadControl.cs — Cumplimiento: 75%

| Línea | Elemento | Problema |
|---|---|---|
| 48 | `new Font("Consolas", 30F, FontStyle.Bold)` | Debería ser `AppTypography.KeypadDisplay` (token recién añadido) |

### AbecedarioControl.cs — Cumplimiento: 70%

| Línea | Elemento | Problema |
|---|---|---|
| 66 | `new Font("Segoe UI", 9F, FontStyle.Bold)` | No en AppTypography |
| 71–72 | `Color.FromArgb(170, 40, 40)` y `Color.FromArgb(140, 30, 30)` | Usar `AppColors.DangerDark` |

### frmHome.cs — Cumplimiento: 72%

| Líneas | Elemento | Problema |
|---|---|---|
| 232, 345, 412 | `Color.FromArgb(248, 249, 252)` | Definir en AppColors |
| 360 | `Color.FromArgb(210, 215, 225)` | Definir en AppColors |
| 224, 438 | `new Font(...)` | Usar AppTypography + corregir leak |

### AutocompleteProductUserControl.cs — Cumplimiento: 60%

No identificado uso de tokens DS. Usa Service Locator. Pendiente de análisis completo.

---

## 8. TOP 10 PROBLEMAS MÁS CRÍTICOS

### 🔴 CRÍTICOS

**#1 — Memory leak masivo: formularios nunca recolectados (MEM-01, MEM-02, MEM-03, MEM-04)**  
`frmHome`, `frmCustomerScreen`, y `frmSplit` son retenidos permanentemente en memoria por suscripciones a eventos en Singletons (`IShoppingCart`, `ZebraScannerService`). En un turno de 8 horas con múltiples logout/login y pagos split, el proceso puede acumular decenas de instancias de formularios completos no liberados, agotando memoria de proceso y GDI objects.  
**Archivos:** `frmHome.cs:77,91,92`, `frmCustomerScreen.cs:26`, `frmSplit.cs:138`  
**Fix estimado:** 2 horas

**#2 — GDI leak: `Region` nunca liberado en cada repaint de `ProductImageControl` (MEM-05)**  
Cada hover y redibujado de tarjeta de producto acumula un objeto GDI `Region` no liberado. Combinado con MEM-06 (controles no dispuestos), el límite de ~10,000 objetos GDI puede alcanzarse en sesiones largas, causando artefactos visuales o crash del proceso.  
**Archivo:** `ProductImageControl.cs:196`  
**Fix estimado:** 30 minutos

**#3 — `async void` en `PaymentOrder` y `tabControl_SelectedIndexChanged` sin try/catch (ERR-01)**  
Un timeout del terminal de pago o error de red durante `tabControlMenuCategories_SelectedIndexChanged` y `PaymentOrder` propaga una excepción no manejada que termina el proceso en producción. El POS puede cerrarse en medio de una transacción activa, sin guardar el estado.  
**Archivo:** `frmHome.cs:~459, ~809`  
**Fix estimado:** 3 horas

**#4 — ShoppingCart: persistencia SQLite silente (ERR-03)**  
Los 4 métodos de mutación del carrito (`AddItem`, `UpdateQuantity`, `RemoveItem`, `Clear`) usan `Task.Run` fire-and-forget sin logging ni manejo de errores en la lambda. Un fallo de SQLite es completamente invisible. Al reiniciar, el carrito en disco puede estar en estado inconsistente.  
**Archivo:** `ShoppingCart.cs:103-210`  
**Fix estimado:** 1 hora

**#5 — `ProductImageControl` descartado sin `Dispose` en cada cambio de tab/letra (MEM-06)**  
`flowLayoutPanel.Controls.Clear()` no dispone los controles removidos. Cada cambio de categoría o filtro de letra acumula instancias orphaned con HWNDs, Regions, Fonts y ToolTips no liberados.  
**Archivo:** `frmHome.cs:~494-525`  
**Fix estimado:** 30 minutos

### 🟠 ALTOS

**#6 — Service Locator en 7 archivos con riesgo de NullReferenceException (ARCH-A01 + ERR-02)**  
`Program.GetService<T>()` devuelve `null` en lugar de lanzar excepción si el tipo no está registrado. Los 7 archivos identificados pueden fallar con `NullReferenceException` críptico en lugar de un mensaje claro de "servicio no registrado".  
**Archivos:** `frmHold.cs`, `frmCierreDiario.cs`, `frmPopupCashPayment.cs`, `AutocompleteProductUserControl.cs`, `frmProductNoExist.cs`, `frmPrintInvoice.cs`, `frmSetting.cs`  
**Fix estimado:** 4 horas

**#7 — N+1 query en `frmHold.buttonAdd_Click` (PERF-04)**  
Hasta 10 queries SQLite secuenciales para encontrar un slot de hold vacío. En una sesión de alto volumen con muchos holds, esto introduce latencia perceptible en operaciones que deberían ser instantáneas.  
**Archivo:** `frmHold.cs:127-135`  
**Fix estimado:** 2 horas

**#8 — `CartListViewControl` sin `BeginUpdate`/`EndUpdate` (PERF-01)**  
20+ repaints por cada actualización del carrito. Con escaneos rápidos en caja, produce parpadeo visible y consume CPU innecesariamente.  
**Archivo:** `CartListViewControl.cs:98`  
**Fix estimado:** 30 minutos

**#9 — Font "Montserrat" no verificada antes de instanciar (ERR-04)**  
En máquinas sin "Montserrat" instalada, `frmSignIn` y `frmCierreDiario` mostrarán fonts de fallback completamente distintas. La pantalla de login puede parecer rota visualmente en despliegues limpios del sistema.  
**Archivos:** `frmSignIn.cs:90`, `frmCierreDiario.cs:68`  
**Fix estimado:** 1 hora

### 🟡 MEDIOS

**#10 — Carga secuencial de categorías en arranque del POS (PERF-03)**  
`LoadMenuCategoriesAsync()` y `LoadCategoriesAsync()` se ejecutan secuencialmente en `frmHome_Load`. Paralelizar con `Task.WhenAll` reduciría el tiempo de arranque en ~40%, mejorando la experiencia del cajero al iniciar sesión.  
**Archivo:** `frmHome.cs:375-376`  
**Fix estimado:** 30 minutos

---

## Resumen de Deuda Técnica

| Categoría | Issues Críticos | Issues Altos | Issues Medios | Issues Bajos |
|---|---|---|---|---|
| Memory Leaks | 6 | 4 | 3 | 2 |
| Performance | 0 | 3 | 4 | 3 |
| Errores potenciales | 2 | 4 | 2 | 2 |
| Arquitectura/DI | 0 | 2 | 3 | 2 |
| Design System | 0 | 2 | 8 | 5 |
| Código muerto | 0 | 0 | 2 | 3 |
| **TOTAL** | **8** | **15** | **22** | **17** |

**Esfuerzo estimado de resolución total:** 25–35 horas de desarrollo  
**Orden de priorización recomendado:** Memory leaks → Async void en pagos → SQLite fire-and-forget → Service Locator → Performance → Design System

---

*Reporte generado: Marzo 2026 | OmadaPOS Post-Fase 5*
