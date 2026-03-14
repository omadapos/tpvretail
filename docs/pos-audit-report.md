# AUDITORÍA COMPLETA — OmadaPOS Retail/Supermercado

**Fecha:** Marzo 2026  
**Alcance:** Revisión completa del código fuente — servicios, formularios, hardware, seguridad, reportes y arquitectura  
**Sistema:** POS WinForms para retail/supermercado, USA

---

## 1. RESUMEN EJECUTIVO

El sistema es un POS WinForms con una arquitectura técnica razonablemente sólida: separación de servicios, DI configurado, base de clases UI compartida (`POSDialog`, `ElegantButtonStyles`, `ListViewTheme`), integración OPOS para scanner/scale, recibo ESC/POS funcional, y un flujo de venta básico operativo.

**Sin embargo, el sistema NO está listo para producción en un supermercado retail profesional en USA.** Los motivos críticos:

1. **No existe ningún módulo de devoluciones/reembolsos** — ni botón, ni servicio, ni formulario.
2. **La contraseña de login está hardcodeada** como `"12345678"` en el código fuente.
3. **No hay control de roles** — cualquier cajero accede a todo.
4. **El EBT cobra el total del carrito**, no solo ítems EBT-elegibles.
5. **`GiftCardService.PlaceSaldo` siempre retorna `true`** — los débitos fallidos son silenciosos.
6. **La pantalla del cliente muestra "DAILY STOP"** hardcodeado.
7. **No hay inventario, X-report, ni supervisión de hardware.**
8. **El PIN de supervisor es "1234" en memoria** — se resetea a ese valor en cada reinicio.

El sistema puede operar en producción de baja escala con correcciones urgentes en los puntos críticos, pero requiere trabajo significativo antes de ser un POS retail profesional certificable.

---

## 2. INVENTARIO DE FUNCIONALIDADES EXISTENTES

### Login y Usuarios

| Función | Estado |
|---|---|
| PIN de cajero (6 dígitos) + auto-submit | ✅ Completo |
| Overlay animado durante login | ✅ Completo |
| SessionManager con token JWT | ✅ Completo |
| Logout con `SessionManager.Clear()` | ✅ Completo |
| PIN de supervisor (para salir) | ⚠️ Parcial — solo protege la salida, PIN "1234" se resetea |
| Roles de usuario | ❌ No existe |
| Límite de intentos fallidos | ❌ No existe |
| Timeout de sesión inactiva | ❌ No existe |

### Ventas y Carrito

| Función | Estado |
|---|---|
| Scan por UPC (OPOS + USB-HID) | ✅ Completo |
| Venta por unidad | ✅ Completo |
| Venta por peso (scale integrada) | ✅ Completo |
| Búsqueda PLU (teclado numérico) | ✅ Completo |
| Cambio de cantidad | ⚠️ Parcial — no pre-llena la cantidad actual |
| Eliminar ítem del carrito | ✅ Completo |
| Cancelar todo el pedido | ✅ Completo |
| Hold/resume cart (hasta 10 holds) | ✅ Completo |
| Verificación de edad (AAMVA PDF417) | ✅ Completo |
| Custom product (precio libre) | ⚠️ Parcial — el modo "con impuesto" nunca se abre |
| Producto desconocido con lookup API | ⚠️ Parcial — guarda pero no añade al carrito |
| Descuento 30% carrito | ✅ Completo (hardcoded) |
| Promoción N-for-$X | ✅ Completo |
| Cash Discount Program (3.8% crédito) | ✅ Completo |
| Devoluciones/reembolsos | ❌ No existe en ningún lugar |
| Void de línea con auditoría | ❌ No existe |
| Nota en pedido | ❌ No existe |
| Cupones / códigos de descuento | ❌ No existe |

### Pagos

| Función | Estado |
|---|---|
| Pago en efectivo + cambio | ✅ Completo |
| Crédito (PAX terminal) | ✅ Completo |
| Débito (PAX terminal) | ✅ Completo |
| EBT Food | ⚠️ Parcial — cobra total del carrito, no solo elegibles |
| EBT Balance Inquiry | ✅ Completo |
| Gift Card | ⚠️ Parcial — no atómica; `PlaceSaldo` siempre retorna true |
| Split Payment (múltiples métodos) | ⚠️ Parcial — sin gift card, sin surcharge, sin undo |
| WIC | ❌ No existe (campo en modelo, ningún flujo de pago) |
| Check / ACH | ❌ No existe |
| Propina / Gratuity | ❌ No existe |
| Recibo impreso ESC/POS | ✅ Completo |
| Reimpresión de factura | ✅ Completo |
| Void de transacción de terminal | ❌ No existe |
| Cajón de efectivo | ⚠️ Parcial — sin confirmación de apertura, sin reconciliación |

### Hardware

| Función | Estado |
|---|---|
| Scanner Zebra OPOS | ✅ Completo |
| Scale Zebra OPOS (stable/unstable/zero/over) | ✅ Completo |
| Reconexión manual scanner/scale | ✅ Completo |
| Impresora ESC/POS (raw bytes) | ✅ Completo |
| Customer display (segunda pantalla) | ✅ Completo |
| Reconexión automática de hardware | ❌ No existe |
| Panel de diagnóstico de hardware | ❌ No existe |
| Error events de OPOS | ❌ No existe |
| Monitor de estado de impresora | ❌ No existe |

### Reportes

| Función | Estado |
|---|---|
| Cierre diario (Z-Report impreso) | ⚠️ Parcial — sin preview, sin confirmación, sin PIN supervisor |
| Historial de facturas + reimpresión | ✅ Completo |
| X-Report (snapshot sin cerrar) | ❌ No existe |
| Reporte por turno | ❌ No existe |
| Reporte de descuentos | ❌ No existe |
| Reporte de impuestos | ❌ No existe |
| Reporte de inventario | ❌ No existe |
| Exportar CSV/PDF | ❌ No existe |
| Ver auditoría de verificación de edad | ❌ No existe (datos en SQLite local, sin UI) |

### Configuración y Seguridad

| Función | Estado |
|---|---|
| Configuración de terminal (IP/Puerto/Impresora) | ✅ Completo |
| Branch name/footer desde API | ✅ Completo |
| Cash Discount toggle por branch | ✅ Completo |
| Roles y permisos | ❌ No existe |
| Cambio de PIN supervisor | ❌ No existe |
| Audit trail de acciones | ❌ Muy limitado (solo apertura de cajón) |

---

## 3. FUNCIONALIDADES FALTANTES

### Críticas — Bloquean operación profesional

1. **Devoluciones y reembolsos** — Ninguna forma de procesar un retorno de producto, void de venta, o reversal de pago. Cero implementación.
2. **Control de roles** — Sin manager vs. cajero. Cualquier cajero accede a cierre diario, descuentos, configuración.
3. **EBT eligibility filtering** — El pago EBT debe limitarse al subtotal de ítems marcados `IsEBT = true`, no al total del carrito.
4. **Void/cancel de transacción en terminal PAX** — Si una transacción fue aprobada por error, no hay void.
5. **WIC como método de pago** — El campo existe en el modelo pero no hay flujo de pago.

### Importantes — Operación sub-óptima sin ellas

6. **X-Report** — Los cajeros no pueden ver un subtotal sin ejecutar el cierre.
7. **Supervisión de hardware** — Sin panel de estado de scanner/scale/impresora.
8. **Reconexión automática de scanner/scale** — Requiere acción manual.
9. **Split payment con Gift Card** — No está disponible en modo split.
10. **Undo de pago parcial en split** — No se puede eliminar un pago ya aplicado.
11. **Cierre diario con PIN supervisor** — Operación destructiva sin autorización.
12. **Preview del cierre antes de ejecutar** — El cajero cierra "a ciegas".
13. **Reconciliación de cajón** — Sin conteo de efectivo, sin variance report.
14. **Nota de pedido** — Sin campo para instrucciones especiales.
15. **`frmCheckPrice` nunca se abre** — El formulario existe y funciona pero ningún botón lo invoca.
16. **"Quick Sale con impuesto"** — El modo `bTax=true` de `frmProductNew` nunca se abre.
17. **Configuración de tasa de surcharge** — El 3.8% es una constante de compilación.
18. **Producto nuevo no se añade al carrito** — Después de guardar en `frmProductNoExist`, el cajero debe re-escanear.

### Premium / Fase futura

19. Loyalty / puntos de cliente
20. Digital receipt (email/SMS)
21. Multi-store dashboard
22. Inventory management completo
23. Módulo de compras / recepciones
24. Self-checkout
25. Weighted barcode labels (impresión)
26. Coupons / promo codes

---

## 4. PROBLEMAS TÉCNICOS

### Críticos

| # | Problema | Archivo | Impacto |
|---|---|---|---|
| T1 | **Contraseña hardcodeada `"12345678"`** | `frmSignIn.cs:328` | Seguridad crítica |
| T2 | **`GiftCardService.PlaceSaldo` bloque `if` vacío — siempre retorna `true`** | `GiftCardService.cs:68` | Débito de tarjeta nunca confirmado |
| T3 | **Pago tomado pero `PlaceOrder` falla — sin recovery ni void** | `PaymentCoordinatorService.cs:216` | Cobro sin orden registrada |
| T4 | **EBT cobra total del carrito, no solo elegibles** | `PaymentCoordinatorService.cs:200` | Incumplimiento SNAP/EBT |
| T5 | **`CategoryService` y `GiftCardService` race condition en `DefaultRequestHeaders`** | `CategoryService.cs:35`, `GiftCardService.cs:28` | Tokens mezclados entre usuarios |

### Altos

| # | Problema | Archivo | Impacto |
|---|---|---|---|
| T6 | **PIN de supervisor "1234" en memoria, nunca persiste, sin lockout** | `SupervisorConfig.cs` | Seguridad operativa |
| T7 | **Weight barcode crea producto nuevo en servidor en cada scan** | `ProductApplicationService.cs:99` | Catálogo se llena de registros ephemeral |
| T8 | **`BranchId = 31` hardcodeado como default en `LoginResponse`** | `LoginResponse.cs` | Usará branch 31 si API no retorna |
| T9 | **`LoadProductInfoByUPC` hardcodea branch 31 en la URL** | `CategoryService.cs:232` | Busca en branch equivocado |
| T10 | **`PaymentSplitService.Clear()` bloquea UI thread con `GetAwaiter().GetResult()`** | `PaymentSplitService.cs:62` | Deadlock potencial |
| T11 | **Dev UPC `"071537001303"` siempre age-restricted en producción** | `AgeRestrictionConfigService.cs:59` | Falso positivo en caja |
| T12 | **Sin token refresh — JWT expirado silencia todas las API calls** | `UserService.cs` | Sesión "fantasma" sin re-login |
| T13 | **`CategoryId = 761` hardcodeado para productos por peso/custom** | `ProductApplicationService.cs:87–93` | Todos los custom en categoría fija |
| T14 | **`CUSTOMERID = 54` hardcodeado** | `Constants.cs` | Todas las ventas al mismo cliente |
| T15 | **Split payment sin validación de suma = total** | `PaymentCoordinatorService.cs:250` | Puede completar con saldo incorrecto |

### Medios

| # | Problema | Archivo |
|---|---|---|
| T16 | `PlaceOrderMultipleModel` sin campo `Service_Fee` | `OrderApplicationService.cs` |
| T17 | Campos de tarjeta (holder, type, ref) nunca se guardan en la orden | `OrderApplicationService.cs:62–82` |
| T18 | `CREATE_CART_TABLE_SQL` no incluye `RequiresAgeVerification` (solo en migración) | `SqliteManager.cs:52–71` |
| T19 | `DropTablesAsync` no elimina tablas de age-restriction | `SqliteManager.cs:999–1035` |
| T20 | `DiscountPolicy.StandardRate = 0.30` — no configurable | `DiscountPolicy.cs` |
| T21 | `GiftCardService.PlaceRecarga` también bloque vacío, siempre true | `GiftCardService.cs:80` |
| T22 | `BaseUrl` hardcodeado en código fuente — sin staging | `Constants.cs` |
| T23 | Sin retry/circuit-breaker en ninguna API call | Todo el codebase |
| T24 | Sin `IZebraScannerService` — no se puede mockear ni sustituir | `ZebraScannerService.cs` |
| T25 | Sin logging sink — todos los `ILogger` son no-ops en producción | `Program.cs` |
| T26 | Audit de verificación de edad solo en SQLite local, no en servidor | `AgeVerificationService.cs` |
| T27 | Promotion math con `int` cast — falla con pesos decimales | `PricingEngine.cs:79–83` |
| T28 | `ReceiptPrinter.Print()` ignora `AdminSetting.PrinterName` | `ReceiptPrinter.cs` |
| T29 | `EscPosBuilder` usa Latin-1 en lugar de CP437 | `EscPosBuilder.cs` |
| T30 | Crash log path: `Desktop\OmadaPOS_crash.txt` — inapropiado para producción | `Program.cs` |

---

## 5. PROBLEMAS UX/UI

| # | Problema | Formulario | Severidad |
|---|---|---|---|
| U1 | `frmPopupQuantity` no pre-llena la cantidad actual del ítem | `frmPopupQuantity.cs` | Alta |
| U2 | `frmHold` restaura el carrito con un solo clic en la lista (sin confirmación dedicada) | `frmHold.cs` | Alta |
| U3 | `frmCierreDiario` ejecuta sin confirmación ni preview | `frmCierreDiario.cs` | Alta |
| U4 | `frmPaymentStatus` usa acento verde para mensajes de decline | `frmPaymentStatus.cs` | Media |
| U5 | Menú del header en español ("Configuración", "Cerrar sesión") — UI en inglés | `POSHeaderControl.cs` | Media |
| U6 | `frmPaymentWaiting` tiene textos en español ("Procesando pago…") | `frmPaymentWaiting.cs` | Media |
| U7 | Banner en scanner/scale desconectados en español ("Reconectar") | `frmHome.cs` | Media |
| U8 | Customer screen muestra "DAILY STOP" hardcodeado | `frmCustomerScreen.cs` | Alta |
| U9 | `frmGiftCard` sin debounce — 16 API calls al escribir manualmente el código | `frmGiftCard.cs` | Media |
| U10 | Gift card insuficiente: no ofrece pago parcial | `frmGiftCard.cs` | Alta |
| U11 | `frmProductNoExist` no añade el producto al carrito después de guardarlo | `frmProductNoExist.cs` | Alta |
| U12 | Carrito: columna "Product" centrada — debería estar alineada a la izquierda | `CartListViewControl` | Baja |
| U13 | `frmPrintInvoice` retorna solo 50 registros sin indicador de paginación | `frmPrintInvoice.cs` | Media |
| U14 | `frmHold` requiere 10 llamadas API secuenciales para verificar los 10 slots | `frmHold.cs` | Media |
| U15 | No hay "Thank you" screen en customer display tras pago completado | `frmCustomerScreen.cs` | Media |
| U16 | No hay pantalla "Payment in progress" en customer display | `frmCustomerScreen.cs` | Media |
| U17 | `frmCheckPrice` existe pero nunca se abre desde ningún botón | `frmCheckPrice.cs` | Alta |
| U18 | `frmCierreDiario` no requiere PIN de supervisor | `frmCierreDiario.cs` | Alta |
| U19 | `frmSplit` no permite remover un pago ya aplicado | `frmSplit.cs` | Alta |
| U20 | `frmPaymentWaiting` sin botón de cancelar — cajero espera 90 s si terminal muerto | `frmPaymentWaiting.cs` | Media |
| U21 | Cantidad de ítems en customer display tipada como `double` — puede mostrar "3E+02" | `frmCustomerScreen.cs` | Baja |
| U22 | Banners rotan cada 4 s — demasiado rápido para leer contenido promocional | `frmCustomerScreen.cs` | Baja |

---

## 6. MATRIZ DE PRIORIDAD

### 🔴 Prioridad ALTA — Bloquean operación profesional

| ID | Item | Archivo(s) |
|---|---|---|
| A1 | Módulo de devoluciones/reembolsos | Nuevo: `frmRefund`, `IRefundService` |
| A2 | EBT: filtrar por ítems elegibles únicamente | `PaymentCoordinatorService.cs` |
| A3 | `GiftCardService.PlaceSaldo` — validar respuesta HTTP | `GiftCardService.cs` |
| A4 | Producto nuevo → añadir al carrito sin re-escanear | `frmProductNoExist.cs` |
| A5 | Control de roles (cashier / manager) | `SessionManager`, `LoginResponse`, DI |
| A6 | Surcharge en split payment (leg de crédito) | `PaymentCoordinatorService.cs`, `PlaceOrderMultipleModel` |
| A7 | Customer display: branch name desde API | `frmCustomerScreen.cs` |
| A8 | `frmCheckPrice`: conectar botón en UI | `frmHome` o `PaymentPanelControl` |
| A9 | Confirmar cierre diario + PIN supervisor | `frmCierreDiario.cs` |
| A10 | Race condition `CategoryService`/`GiftCardService` HttpClient | `CategoryService.cs`, `GiftCardService.cs` |

### 🟠 Prioridad MEDIA — Debilitan la operación

| ID | Item |
|---|---|
| M1 | X-Report (snapshot sin cierre) |
| M2 | `frmPopupQuantity`: pre-llenar cantidad actual |
| M3 | `frmHold`: botón "Restore" explícito; identificar hold por nombre/nota |
| M4 | `frmCierreDiario`: preview on-screen antes de imprimir |
| M5 | Reconexión automática de scanner/scale (timer de heartbeat) |
| M6 | Panel de estado de hardware en la UI |
| M7 | PIN de supervisor persiste en configuración (cifrado) |
| M8 | Límite de intentos fallidos en login y supervisor PIN |
| M9 | Split: botón "Remove last payment" |
| M10 | Gift card: pago parcial (balance < total) |
| M11 | `frmPaymentStatus`: acento rojo para declines |
| M12 | Unificar idioma: inglés o español, no ambos |
| M13 | Tasa de surcharge configurable (no hardcoded) |
| M14 | Tasa de impuesto por branch/categoría (no hardcoded 7%) |
| M15 | Logging sink (Serilog a archivo) |
| M16 | Retry/timeout configurable en API calls |
| M17 | Token refresh automático antes de expiración |
| M18 | Audit trail en servidor para acciones críticas |
| M19 | `ReceiptPrinter.PrintTo(printerName)` usarlo consistentemente |
| M20 | Corregir encoding `EscPosBuilder` a CP437 para caracteres especiales |

### 🟡 Prioridad BAJA — Mejoran calidad

| ID | Item |
|---|---|
| B1 | `IZebraScannerService` interface |
| B2 | `frmSupervisorPin`: registrar en DI |
| B3 | Eliminar `GetService<T>()` service locator |
| B4 | Eliminar dev UPC hardcodeado `"071537001303"` |
| B5 | Reconciliación de cajón (conteo de efectivo) |
| B6 | Exportar reportes a CSV/PDF |
| B7 | Paginación en `frmPrintInvoice` (más de 50 órdenes) |
| B8 | "Thank you" screen en customer display |
| B9 | `frmPaymentWaiting`: traducir textos al inglés |
| B10 | `CartListViewControl`: alinear columna "Product" a la izquierda |
| B11 | `BaseUrl` configurable sin recompilar (`appsettings.json`) |
| B12 | Test print button en `frmSetting` |
| B13 | Enumeración de impresoras disponibles en `frmSetting` |

### 🔵 Premium / Fase Futura

| Item |
|---|
| Módulo de inventario (stock, recepciones, ajustes) |
| Loyalty / puntos de cliente |
| Digital receipt (email/SMS) |
| Dashboard multi-tienda |
| Self-checkout |
| Coupon codes / promociones por código |
| Módulo de órdenes de compra a proveedores |
| Integración con weight label printer (etiquetas por peso) |
| KDS / Kitchen Display System |
| Offline mode con sync posterior |

---

## 7. RECOMENDACIONES CONCRETAS DE MEJORA

### Seguridad (Urgente)

1. **Eliminar la contraseña hardcodeada.** Usar un campo real de contraseña con bcrypt, o rediseñar el API para aceptar solo el PIN sin password. No enviar `"12345678"` en producción.
2. **Persistir el supervisor PIN** en `AdminSetting` con hash (bcrypt o SHA-256 con salt). Cargarlo en `SessionManager` al inicio junto con el resto de la configuración.
3. **Agregar `RoleId` al `LoginResponse` y `SessionManager`.** Habilitar restricciones de UI según rol.
4. **Bloqueo de intento fallido:** 5 intentos → 30 segundos de bloqueo.

### Flujo de Pago (Urgente)

5. **Devoluciones:** Crear `IRefundService` que llame al PAX terminal con `TransactionType = "Return"` y al API con un endpoint de void/refund. Botón "RETURN" en `PaymentPanelControl` con PIN de supervisor.
6. **EBT elegible:** En `ProcessEBTPaymentAsync`, calcular `EbtTotal = cart.Items.Where(i => i.IsEBT).Sum(i => i.Total)` y enviar ese monto al terminal.
7. **`GiftCardService`:** Validar `response.IsSuccessStatusCode` antes de retornar `true`.

### Catálogo y Productos (Alto)

8. **Productos por peso:** No crear registro de servidor en cada scan. Usar un producto "Bulk Item" genérico con un ID fijo por categoría de peso. El nombre y precio del scan van en el comentario de línea.
9. **`frmProductNoExist`:** Después de `OnConfirmAsync`, emitir un evento que regrese el UPC al `HomeInitializationService` para re-buscar y añadir al carrito automáticamente.

### Reportes (Medio)

10. **X-Report:** Reusar `CierreDiario` con un flag `preview = true` que no ejecute el close, solo retorne los totales del día para mostrarlos en pantalla.
11. **Preview de cierre:** Cargar y mostrar un resumen (ventas, transacciones, métodos) antes de confirmar el cierre.

### Hardware (Medio)

12. **Heartbeat timer:** En `ZebraScannerService`, lanzar un `Timer` de 30 s que llame a `TryReconnectScanner()` / `TryReconnectScale()` si `IsConnected == false`. Emitir un evento cuando se reconecta automáticamente.
13. **Diagnóstico visible:** Añadir íconos de estado (🟢/🔴) en `POSHeaderControl` para scanner, scale, e impresora. Actualizarlos vía eventos del `ZebraScannerService`.

### Arquitectura (Medio-Largo)

14. **`appsettings.json`:** Mover `BaseUrl`, `Constants.IP`, `Constants.PORT`, surcharge rate, discount rate a configuración externa leída con `IConfiguration`.
15. **Logging:** Agregar Serilog con sink a archivo rotatorio diario en `%LocalAppData%\OmadaPOS\logs\`.
16. **Retry policy:** Envolver todas las llamadas API con `Polly` (3 reintentos con backoff exponencial).

---

## 8. ROADMAP POR FASES

### FASE 1 — Estabilización (2–4 semanas)

*Objetivo: El sistema puede operar en producción de forma segura y sin errores silenciosos graves.*

| Task | Prioridad |
|---|---|
| Corregir `GiftCardService.PlaceSaldo` — validar respuesta HTTP | Crítica |
| Race condition HttpClient en `CategoryService`/`GiftCardService` | Crítica |
| EBT: limitar al subtotal de ítems elegibles | Crítica |
| Surcharge en split payment | Alta |
| Producto nuevo → auto-añadir al carrito tras guardar | Alta |
| `frmCheckPrice`: abrir desde un botón en la UI | Alta |
| Dev UPC hardcodeado: eliminar de `AgeRestrictionConfigService` | Alta |
| Branch hardcodeado 31 en `CategoryService.LoadProductInfoByUPC` | Alta |
| `ReceiptPrinter.Print()` — usar siempre `PrintTo(printerName)` | Alta |
| Unificar idioma (inglés) en menú header, `frmPaymentWaiting` | Media |
| Customer display: cargar nombre de branch desde `BranchModel` | Media |
| `frmPopupQuantity`: pre-llenar cantidad actual | Media |
| Logging sink a archivo (Serilog) | Media |
| Eliminar `"Daily Stop"` hardcodeado en `frmCustomerScreen` | Media |

### FASE 2 — Operación Retail Profesional (4–8 semanas)

*Objetivo: Un cajero de supermercado puede operar el sistema sin recurrir a procedimientos manuales externos.*

| Task |
|---|
| Módulo de devoluciones/reembolsos (PAX Return + API void) |
| Control de roles: cashier / manager desde `LoginResponse.RoleId` |
| PIN de supervisor: persistir en `AdminSetting`, cambiar desde `frmSetting` |
| Bloqueo de intentos fallidos (login y supervisor) |
| Cierre diario: PIN de supervisor + preview de totales |
| X-Report (snapshot sin cierre) |
| Reconciliación de cajón (conteo de efectivo) |
| `frmSplit`: botón "Remove last payment"; incluir Gift Card |
| Heartbeat y reconexión automática de scanner/scale |
| Panel de diagnóstico de hardware visible en header |
| Retry policy en API calls (Polly) |
| Token refresh automático |
| `BaseUrl` y parámetros sensibles a `appsettings.json` |
| Configurar tasa de surcharge y tasa de impuesto por branch |
| Audit trail en servidor para acciones críticas |

### FASE 3 — Expansión (2–6 meses)

*Objetivo: POS competitivo con funciones avanzadas de grocery retail.*

| Task |
|---|
| Módulo de inventario (stock, alertas, recepciones) |
| Coupon codes / promociones avanzadas (BOGO, % off) |
| Loyalty / puntos de cliente |
| Digital receipt (email) |
| Dashboard de reportes (ventas diarias/semanales, top productos) |
| Módulo de órdenes de compra |
| Exportar reportes a CSV/PDF |
| Multi-store (filtros por branch en reportes) |
| Weighted barcode label printer integration |
| Offline mode con sync posterior |

---

## 9. ARCHIVOS PRIORITARIOS PARA INTERVENIR PRIMERO

| Orden | Archivo | Razón |
|---|---|---|
| 1 | `OmadaPOS.Libreria/Services/GiftCardService.cs` | Bug crítico silencioso — siempre retorna true |
| 2 | `OmadaPOS.Libreria/Services/CategoryService.cs` | Race condition HttpClient |
| 3 | `OmadaPOS/Services/POS/PaymentCoordinatorService.cs` | EBT elegibles, surcharge en split |
| 4 | `OmadaPOS/Views/frmProductNoExist.cs` | Producto guardado no se añade al carrito |
| 5 | `OmadaPOS/Services/AgeRestrictionConfigService.cs` | Dev UPC en producción |
| 6 | `OmadaPOS.Libreria/Services/CategoryService.cs:232` | Branch 31 hardcodeado |
| 7 | `OmadaPOS/Views/frmCustomerScreen.cs` | "DAILY STOP" hardcodeado (2 lugares) |
| 8 | `OmadaPOS/Impresora/ReceiptPrinter.cs` | Usar `PrintTo(printerName)` consistentemente |
| 9 | `OmadaPOS/Views/frmCierreDiario.cs` | Añadir confirmación + PIN supervisor |
| 10 | `OmadaPOS/Views/frmSetting.cs` | Añadir campo de supervisor PIN configurable |
| 11 | `OmadaPOS/Services/SupervisorConfig.cs` | Persistencia y hashing del PIN |
| 12 | `OmadaPOS/Views/frmPaymentWaiting.cs` | Traducir textos españoles al inglés |
| 13 | `OmadaPOS/Presentation/Controls/POSHeaderControl.cs` | Traducir menú al inglés |
| 14 | `OmadaPOS/Views/frmPopupQuantity.cs` | Pre-llenar cantidad actual |
| 15 | `OmadaPOS/Program.cs` | Agregar Serilog, mover `BaseUrl` a config |

---

## 10. RIESGOS EN PRODUCCIÓN SIN CORREGIR

| Riesgo | Consecuencia Operativa | Severidad |
|---|---|---|
| **Sin devoluciones/reembolsos** | Cualquier retorno requiere procedimiento manual externo. Los cajeros darán el cambio "de caja" sin registro, creando discrepancias de cierre diario que no cuadran con los totales del sistema. | 🔴 Crítico |
| **EBT cobra total, no solo elegibles** | El terminal puede rechazar el pago cuando el balance EBT no cubre artículos no elegibles. Cashiers quedarán trabados sin entender por qué. Posible violación de USDA SNAP regulation. | 🔴 Crítico |
| **`GiftCardService.PlaceSaldo` siempre true** | Un cliente puede usar una gift card caducada, bloqueada, o con saldo 0. El POS reportará éxito. El balance nunca se deduce. Riesgo de fraude y pérdida de revenue. | 🔴 Crítico |
| **Sin roles** | Cualquier cajero puede ejecutar cierre diario, aplicar el 30% de descuento, abrir el cajón, acceder a configuración. Sin control de operaciones sensibles. | 🟠 Alto |
| **PIN supervisor "1234" en memoria** | Tras cada reinicio el PIN vuelve a "1234". Cualquier empleado puede descubrirlo y autorizar salidas no autorizadas. Sin lockout, trivialmente iterable. | 🟠 Alto |
| **Pago tomado, orden API falla** | Si el backend está caído al momento del checkout, el terminal cobra la tarjeta pero no se registra la venta. El dinero llega a la cuenta pero no aparece en reportes. | 🟠 Alto |
| **Dev UPC hardcodeado** | El UPC `"071537001303"` siempre pedirá verificación de edad, causando fricción innecesaria en cada venta de ese producto. | 🟡 Medio |
| **Sin logging en producción** | Todos los `ILogger` son no-ops. Si algo falla, no hay rastro. Diagnóstico requeriría reproducir el problema con un desarrollador presente. | 🟡 Medio |
| **Branch 31 hardcodeado** | Si el branch ID del usuario logueado no es 31, la búsqueda de productos por UPC falla silenciosamente o retorna datos de otra tienda. | 🟡 Medio |
| **Sin token refresh** | Después de X horas, todas las API calls retornan 401. El sistema deja de buscar productos, registrar ventas, hacer payouts — todo falla silenciosamente sin re-login. | 🟡 Medio |
| **Sin reconexión automática de hardware** | Un cable de scanner desconectado mid-shift requiere acción manual. No hay alerta visible. El cashier simplemente "no puede escanear" sin saber por qué. | 🟡 Bajo-Medio |
| **Encoding Latin-1 en recibos** | Caracteres como `á, é, ñ, ü` en nombres de productos o dirección de tienda se imprimirán como caracteres incorrectos o cajas negras. | 🟢 Bajo |

---

## HALLAZGOS POR MÓDULO — RESUMEN RÁPIDO

| Módulo | Estado | Notas clave |
|---|---|---|
| Login | ⚠️ Funcional con riesgos | Contraseña hardcoded, sin lockout, sin roles |
| Main POS | ✅ Funcional | Sin returns, sin `frmCheckPrice` activo |
| Product lookup | ✅ Funcional | |
| Customer lookup | ❌ No existe | `CUSTOMERID = 54` siempre |
| Payment — Cash | ✅ Funcional | |
| Payment — Credit/Debit | ✅ Funcional | Surcharge correcto en pago único |
| Payment — EBT | ⚠️ Parcial | Cobra total, no solo elegibles |
| Payment — Gift Card | ⚠️ Riesgo crítico | `PlaceSaldo` nunca confirma el débito |
| Payment — Split | ⚠️ Parcial | Sin gift card, sin surcharge, sin undo |
| Returns/Refunds | ❌ No existe | |
| Scale integration | ✅ Funcional | Sin reconexión automática |
| Scanner integration | ✅ Funcional | Sin reconexión automática, sin error events |
| Customer display | ⚠️ Parcial | Store name hardcoded, sin thank-you screen |
| Inventory | ❌ No existe | |
| Reports — Daily Close | ⚠️ Parcial | Sin preview, sin PIN supervisor |
| Reports — Invoice History | ✅ Funcional | Limitado a 50 registros |
| Reports — X-Report | ❌ No existe | |
| Settings/Configuration | ⚠️ Parcial | Sin PIN supervisor configurable, sin test print |
| Security/Roles | ❌ No existe | |
| Shift/Cash Management | ❌ No existe | |
| Hardware Diagnostics | ❌ No existe | |
| Audit Trail | ⚠️ Muy limitado | Solo apertura de cajón registrada en servidor |

---

*Reporte generado con revisión completa del código fuente. Todos los hallazgos verificados directamente en los archivos fuente.*
