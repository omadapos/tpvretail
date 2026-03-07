# OmadaPOS — Reporte de Auditoría de Riesgo Crítico
**Fecha:** 2026-03-06  
**Alcance:** Todos los archivos `.cs` del proyecto OmadaPOS (excluye `.Designer.cs`)  
**Categorías auditadas:** Memory Leaks por eventos · GDI Leaks · `async void` sin `try/catch`

---

## Resumen Ejecutivo

| Categoría | Problemas encontrados | Críticos | Altos | Medios |
|---|---|---|---|---|
| Memory Leaks (eventos) | 3 | 0 | 1 | 2 |
| GDI Leaks | 7 | **4** | 1 | 2 |
| `async void` sin try/catch | 19 | 0 | 15 | 4 |
| **TOTAL** | **29** | **4** | **16** | **6** |

**Formulario / control más crítico:** `UserSessionControl.cs` + `CartListViewControl.cs` + `frmCustomerScreen.cs`  
(Destruyen fonts compartidas del Design System en cada repintado)

**Prioridad máxima sugerida:** Corregir los 4 bugs P0 GDI antes de cualquier otra cosa — provocan corrupción visual silenciosa en producción.

---

## 1. MEMORY LEAKS — Suscripción a Eventos de Singletons

### ✅ Correctamente gestionados

| Archivo | Línea subscripción | Evento | Desuscripción en | Tipo handler |
|---|---|---|---|---|
| `frmHome.cs` | 77 | `_shoppingCart.CartChanged` | `frmHome_FormClosing` (L.405) | Método nombrado ✅ |
| `frmHome.cs` | 91 | `_zebraScannerService.OnBarcodeDataReceived` | `frmHome_FormClosing` (L.409) | Método nombrado ✅ |
| `frmHome.cs` | 92 | `_zebraScannerService.OnWeightUpdated` | `frmHome_FormClosing` (L.410) | Método nombrado ✅ |
| `frmCustomerScreen.cs` | 26 | `_shoppingCart.CartChanged` | `OnFormClosing` (L.371) | Método nombrado ✅ |
| `frmCustomerScreen.cs` | 36 | `SharedData.WeightUnitChanged` | `OnFormClosing` (L.373) | Método nombrado ✅ |
| `frmSplit.cs` | 155 | `_shoppingCart.CartChanged` | `FrmSplit_FormClosed` (L.88) | Lambda almacenada en `_cartChangedHandler` ✅ |

---

### ⚠️ Problemas encontrados

#### ISSUE-M1 · `frmHome.cs` líneas 101–103 · **Riesgo: BAJO**

```csharp
// frmHome.cs L.101-103
_userSessionControl.SettingsRequested   += (_, _) => buttonSetting_Click(this, EventArgs.Empty);
_userSessionControl.DailyCloseRequested += (_, _) => labelCashier_ClickInternal();
_userSessionControl.LogoutRequested     += async (_, _) => await EjecutarLogout();
```

- **Tipo:** Lambdas anónimas — no desuscribibles
- **Fuente del evento:** `_userSessionControl` (control hijo de frmHome, **no** singleton)
- **Desuscripción:** Ninguna
- **Riesgo real:** BAJO — el control tiene el mismo ciclo de vida que el formulario; mueren juntos
- **Riesgo adicional:** La lambda de la línea 103 es `async (_, _) =>` sin `try/catch`; si `EjecutarLogout()` lanza, la excepción no se captura
- **Acción correctiva:** Convertir a métodos nombrados y agregar try/catch en la lambda async, o moverlo al `UserSessionControl` directamente

---

#### ISSUE-M2 · `UserSessionControl.cs` líneas 39–40 · **Riesgo: MEDIO**

```csharp
// UserSessionControl.cs L.39-40
_ownerForm.MouseClick      += (_, _) => HideMenu();
_dismissSurface.MouseClick += (_, _) => HideMenu();
```

- **Tipo:** Lambdas anónimas suscritas al formulario padre (`_ownerForm = frmHome`)
- **Desuscripción:** Ninguna — no hay `Dispose(bool)` sobreescrito en `UserSessionControl`
- **Riesgo real:** MEDIO — durante la secuencia de dispose, `_ownerForm.MouseClick` mantiene referencias al control ya liberado hasta que el GC recoge frmHome completo
- **Acción correctiva:**

```csharp
// En UserSessionControl.cs — agregar:
private EventHandler? _ownerClickHandler;
private EventHandler? _dismissClickHandler;

// En el constructor/init reemplazar:
_ownerClickHandler   = (_, _) => HideMenu();
_dismissClickHandler = (_, _) => HideMenu();
_ownerForm.MouseClick      += _ownerClickHandler;
_dismissSurface.MouseClick += _dismissClickHandler;

// Agregar override Dispose:
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        if (_ownerForm != null)     _ownerForm.MouseClick      -= _ownerClickHandler;
        if (_dismissSurface != null) _dismissSurface.MouseClick -= _dismissClickHandler;
    }
    base.Dispose(disposing);
}
```

---

#### ISSUE-M3 · `frmHold.cs` líneas 29–36 · **Riesgo: BAJO**

```csharp
// frmHold.cs L.29-36
_holdService            = Program.GetService<IHoldService>();
_shoppingCart           = Program.GetService<IShoppingCart>();
_homeInteractionService = Program.GetService<IHomeInteractionService>();
```

- **Tipo:** Service Locator anti-pattern (no DI constructor)
- **Leak de eventos:** `frmHold` NO se suscribe a eventos de los singletons — sin leak de eventos
- **Riesgo real:** BAJO para estabilidad, MEDIO para arquitectura
- **Acción correctiva:** Migrar a constructor DI (igual que se hizo con `frmSignIn`, `frmSplit`, `frmCustomerScreen`)

---

## 2. GDI LEAKS — Recursos IDisposable no gestionados

### 🔴 CRÍTICO — Fonts compartidas de AppTypography destruidas en cada repintado

> **Contexto crítico:** `AppTypography` define todas sus fuentes como `static readonly Font` — instancias compartidas de toda la aplicación. Si se envuelven en `using`, se llama `Dispose()` en la instancia compartida después del primer uso, haciendo inválido el handle GDI+ para **todos** los dibujos posteriores.

---

#### ISSUE-G1 · `frmCustomerScreen.cs` línea ~146 · **Riesgo: CRÍTICO**

```csharp
// ❌ INCORRECTO — Destruye AppTypography.ColumnHeader en el primer draw
using var hFont  = AppTypography.ColumnHeader;
using var tBrush = new SolidBrush(AppColors.TextWhite);
g.DrawString(e.Header.Text, hFont, tBrush, ...);
```

- **Ubicación:** Handler `DrawColumnHeader` del ListView — se invoca en **cada repintado de encabezados**
- **Efecto:** Tras el primer renderizado, `AppTypography.ColumnHeader` queda disposed. Todos los dibujos posteriores usan un handle GDI inválido → texto invisible o `ArgumentException: Parameter is not valid`
- **Acción correctiva:**

```csharp
// ✅ CORRECTO — Nunca hacer Dispose de un Font estático compartido
var hFont = AppTypography.ColumnHeader;   // sin using
using var tBrush = new SolidBrush(AppColors.TextWhite);  // SolidBrush sí se crea nuevo → sí usar using
g.DrawString(e.Header.Text, hFont, tBrush, ...);
```

---

#### ISSUE-G2 · `CartListViewControl.cs` línea ~78 · **Riesgo: CRÍTICO**

```csharp
// ❌ INCORRECTO — Destruye AppTypography.ColumnHeader en el primer draw del cart ListView
_listView.DrawColumnHeader += (s, e) =>
{
    using var headerFont = AppTypography.ColumnHeader;  // ← GDI LEAK
    using var textBrush  = new SolidBrush(AppColors.TextWhite);
    g.DrawString(e.Header?.Text ?? string.Empty, headerFont, textBrush, textRect, sf);
};
```

- **Efecto:** Mismo que ISSUE-G1, en el ListView del carrito. Los dos controles compiten por destruir el mismo `Font` estático
- **Acción correctiva:** Quitar `using` de `headerFont` (igual que G1)

---

#### ISSUE-G3 · `UserSessionControl.cs` línea ~219 · **Riesgo: CRÍTICO**

```csharp
// ❌ INCORRECTO — Destruye AppTypography.AppHeader en el primer repintado del botón de usuario
private void UserButton_Paint(object? sender, PaintEventArgs e)
{
    using var inicialFont = AppTypography.AppHeader;   // ← GDI LEAK CRÍTICO
    using var nameBrush   = new SolidBrush(AppColors.TextWhite);
    g.DrawString(inicial, inicialFont, inicialBrush, avatarRect, sfCenter);
    ...
}
```

- **Efecto:** Tras la primera vez que el botón se pinta (hover, focus, invalidación), `AppTypography.AppHeader` queda disposed — la letra inicial del cashier desaparece, y además se corrompe el mismo Font usado por `POSHeaderControl`
- **Acción correctiva:** Quitar `using` de `inicialFont`

---

#### ISSUE-G4 · `UserSessionControl.cs` línea ~228 · **Riesgo: CRÍTICO**

```csharp
// ❌ INCORRECTO — Destruye AppTypography.BodySmall en el primer repintado
    using var nameFont = AppTypography.BodySmall;   // ← GDI LEAK CRÍTICO
    g.DrawString(nombre, nameFont, nameBrush, nameRect, sfLeft);
```

- **Efecto:** Idéntico a G3. El nombre del cajero desaparece tras el primer render
- **Acción correctiva:** Quitar `using` de `nameFont`

> **Regla universal:** `SolidBrush`, `Pen`, `StringFormat`, `GraphicsPath` creados localmente → siempre `using`.  
> `Font` de `AppTypography.*` → **NUNCA** `using` (son singletons estáticos compartidos).

---

#### ISSUE-G5 · `frmHome.cs` línea ~167 · **Riesgo: ALTO**

```csharp
// frmHome.cs — ConfigureUI()
var toolTip = new ToolTip { ShowAlways = true };
toolTip.SetToolTip(buttonInvoice,    "Reimprimir último recibo");
toolTip.SetToolTip(DualScreenButton, "Activar pantalla del cliente");
toolTip.SetToolTip(buttonClose,      "Salir / Cerrar sesión");
```

- **Tipo:** `ToolTip` (mantiene `HWND` Win32)
- **Disposed:** No — variable local, no almacenada, no agregada a `components`
- **Efecto:** El handle Win32 del tooltip sobrevive hasta que el GC finaliza el objeto (no determinístico)
- **Acción correctiva:**

```csharp
// Opción A: Agregar a components (patrón WinForms estándar)
var toolTip = new ToolTip { ShowAlways = true };
components ??= new System.ComponentModel.Container();
components.Add(toolTip);

// Opción B: Almacenar como campo y disponer en FormClosing
private ToolTip? _mainTooltip;
// en ConfigureUI(): _mainTooltip = new ToolTip { ... };
// en frmHome_FormClosing: _mainTooltip?.Dispose();
```

---

#### ISSUE-G6 · `OmadaposLogo.cs` línea ~34 · **Riesgo: MEDIO** (performance, no leak real)

```csharp
// WatermarkOmadaPOS.OnPaint — se ejecuta en cada repintado
using (Font fuente = new Font("Montserrat", 32F, FontStyle.Bold, GraphicsUnit.Point))
using (SolidBrush sombra = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
using (SolidBrush textoPrincipal = new SolidBrush(Color.FromArgb(40, 255, 255, 255)))
```

- **Tipo:** Font creado en cada `OnPaint` (correctamente disposed con `using`)
- **Leak:** No — se dispone correctamente
- **Problema:** Cada repintado del logo asigna un nuevo handle GDI+ para el Font. Alta frecuencia = presión innecesaria en el pool GDI
- **Acción correctiva:** Cachear como campo estático o de instancia y disponer en `Dispose(bool)`:

```csharp
private static readonly Font _logoFont = new("Montserrat", 32F, FontStyle.Bold, GraphicsUnit.Point);
// Eliminar el new Font(...) del OnPaint
```

---

#### ISSUE-G7 · `frmCustomerScreen.cs` / panel_Paint handlers · ✅ LIMPIO

Los handlers `panel1_Paint` y `Panel3_Paint` gestionan correctamente la `Region`:

```csharp
var oldRegion = panel.Region;
panel.Region  = new Region(path);
oldRegion?.Dispose();   // ✅ Dispose explícito del anterior
```

---

## 3. async void SIN try/catch

### 🔴 Métodos sin ninguna protección — Riesgo: ALTO

Cualquier excepción no capturada en un `async void` se convierte en una excepción no manejada del thread UI, lo que termina la aplicación en producción (a menos que haya un handler global en `Application.ThreadException`).

| # | Archivo | Línea aprox. | Método | Peligro principal |
|---|---|---|---|---|
| 1 | `frmProductNoExist.cs` | 22 | `frmProductNoExist_Load` | HTTP falla → crash en Load |
| 2 | `frmProductNoExist.cs` | 39 | `buttonSave_Click` | HTTP falla al guardar → crash |
| 3 | `frmPrintInvoice.cs` | 21 | `frmPrintInvoice_Load` | DB falla → crash al abrir |
| 4 | `frmPrintInvoice.cs` | 138 | `PrintInvoice(int orderId)` | Impresora falla → crash |
| 5 | `frmPrintInvoice.cs` | 160 | `buttonSearch_Click` | DB falla → crash |
| 6 | `frmPrintInvoice.cs` | 187 | `listViewInvoices_SelectedIndexChanged` | DB falla → crash |
| 7 | `frmCierreDiario.cs` | 108 | `buttonClose_Click` | Cierre diario falla → crash (operación crítica de negocio) |
| 8 | `frmPopupCashPayment.cs` | 48 | `buttonPrint_Click` | Impresora falla post-venta → crash |
| 9 | `frmSetting.cs` | 20 | `buttonSave_Click` | DB falla → crash al guardar config |
| 10 | `frmGiftCard.cs` | 94 | `textBoxCode_TextChanged` | Falla en cada tecla → crash continuo |
| 11 | `frmKeyLookup.cs` | 43 | `buttonOK_Click` | DB falla → crash en búsqueda |
| 12 | `frmProductNew.cs` | 45 | `buttonOK_Click` | HTTP falla al agregar producto → crash |
| 13 | `frmSplit.cs` | 454 | `buttonEbtBalance_Click` | API EBT falla → crash en pago |
| 14 | `frmCustomerScreen.cs` | 270 | `LoadCart()` | DB falla en constructor → crash al abrir pantalla cliente |
| 15 | `AutocompleteProductUserControl.cs` | 21 | `textBoxSearch_TextChanged` | API falla en cada keystroke → crash |

---

### Detalle de los casos más críticos

#### ISSUE-A1 · `frmCierreDiario.cs` L.108 — CRÍTICO DE NEGOCIO

```csharp
// ❌ Sin try/catch — operación de cierre diario
private async void buttonClose_Click(object sender, EventArgs e)
{
    var cierre = await orderService.CierreDiario(sDate, SessionManager.UserName);
    if (cierre != null)
    {
        var branchInfo = await branchService.LoadBranch(SessionManager.BranchId ?? 0);
        var ticket = new TicketCierre(cierre, branchInfo.Address, branchInfo.Name);
        ticket.print();
        this.Close();
    }
}
```

Si falla la conexión a la API durante el cierre diario, la aplicación se cae sin imprimir el ticket. El operador no sabe si el cierre se procesó o no.

**Corrección:**
```csharp
private async void buttonClose_Click(object sender, EventArgs e)
{
    try
    {
        buttonClose.Enabled = false;
        var cierre = await orderService.CierreDiario(sDate, SessionManager.UserName);
        if (cierre != null)
        {
            var branchInfo = await branchService.LoadBranch(SessionManager.BranchId ?? 0);
            var ticket = new TicketCierre(cierre, branchInfo?.Address ?? "", branchInfo?.Name ?? "");
            ticket.print();
            this.Close();
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error al procesar cierre diario: {ex.Message}\n\nEl cierre puede haberse registrado. Contacte soporte.",
            "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    finally
    {
        buttonClose.Enabled = true;
    }
}
```

---

#### ISSUE-A2 · `frmGiftCard.cs` L.94 — ALTA FRECUENCIA

```csharp
// ❌ Sin try/catch — se invoca en cada keystroke del campo de código
private async void textBoxCode_TextChanged(object sender, EventArgs e)
{
    var code = textBoxCode.Text;
    if (code.Length >= 4)
    {
        var giftCard = await _giftCardService.GetByCode(cardCode);  // falla → crash
        ...
    }
}
```

Un error de red mientras el usuario escribe el código de gift card crashea la app.

**Corrección:**
```csharp
private async void textBoxCode_TextChanged(object sender, EventArgs e)
{
    try
    {
        var code = textBoxCode.Text;
        if (code.Length >= 4)
        {
            var giftCard = await _giftCardService.GetByCode(code.Trim());
            // resto del código...
        }
    }
    catch (Exception ex)
    {
        labelBalance.Text = "Error al verificar tarjeta";
        // Opcional: _logger.LogError(ex, "GiftCard lookup failed");
    }
}
```

---

#### ISSUE-A3 · `AutocompleteProductUserControl.cs` L.21 — ALTA FRECUENCIA

```csharp
// ❌ Sin try/catch — se invoca en cada keystroke ≥ 3 chars
private async void textBoxSearch_TextChanged(object sender, EventArgs e)
{
    if (text.Length >= 3)
        _currentSuggestions = await _categoryService.Autocomplete(text) ?? [];
}
```

**Corrección:**
```csharp
private async void textBoxSearch_TextChanged(object sender, EventArgs e)
{
    try
    {
        var text = textBoxSearch.Text.Trim();
        if (text.Length >= 3)
            _currentSuggestions = await _categoryService.Autocomplete(text) ?? [];
        // resto...
    }
    catch (Exception ex)
    {
        _currentSuggestions = [];
        // Silencioso es aceptable aquí — no mostrar MessageBox en cada keystroke
        // _logger?.LogWarning(ex, "Autocomplete failed for query '{Text}'", textBoxSearch.Text);
    }
}
```

---

#### ISSUE-A4 · `frmCustomerScreen.cs` L.270 — RIESGO EN STARTUP

```csharp
// ❌ Sin try/catch — llamado desde el constructor
private async void LoadCart() => await _shoppingCart.LoadCartAsync();
```

Si `LoadCartAsync` lanza durante la apertura del Customer Screen, la pantalla del cliente queda en estado inválido sin mensaje de error.

**Corrección:**
```csharp
private async void LoadCart()
{
    try
    {
        await _shoppingCart.LoadCartAsync();
    }
    catch (Exception ex)
    {
        // Logger o fallback silencioso — la pantalla debe abrirse aunque el carrito falle
        System.Diagnostics.Debug.WriteLine($"LoadCart failed: {ex.Message}");
    }
}
```

---

### ⚠️ async void público — Riesgo MEDIO

Los callers no pueden `await` un `async void` ni capturar sus excepciones:

| Archivo | Línea | Método | Estado try/catch |
|---|---|---|---|
| `frmHome.cs` | ~773 | `public async void addCustomProduct(...)` | ✅ Tiene try/catch |
| `frmHome.cs` | ~895 | `public async void SearchProduct(string upc)` | ✅ Tiene try/catch |
| `frmHome.cs` | ~946 | `public async void ProcessPaymentMultiple()` | ✅ Tiene try/catch |
| `frmCheckPrice.cs` | ~64 | `public async void SearchProduct(string upc)` | ⚠️ Parcial (try/catch interno pero no en el método raíz) |

**Recomendación:** Convertir todos los `async void` públicos a `async Task` y exponer wrappers `async void` privados para los eventos, o usar el patrón `fire-and-forget` con `_ = MethodAsync().ContinueWith(...)`.

---

### ✅ async void correctamente protegidos

| Archivo | Método |
|---|---|
| `frmHome.cs` | `frmHome_Load`, `tabControlMenuCategories_SelectedIndexChanged`, `buttonLogout_Click`, `buttonOpenDrawer_Click`, `buttonPayCash_Click`, `addCustomProduct`, `SearchProduct`, `ProcessPaymentMultiple`, `PaymentOrder`, `pictureBoxPesado_Click` |
| `frmCustomerScreen.cs` | `LoadData` |
| `frmSplit.cs` | `PaymentButton_Click`, `buttonPrintBill_Click` |
| `frmHold.cs` | `frmHold_Load`, `listBoxHold_SelectedIndexChanged`, `buttonAdd_Click` |
| `frmGiftCard.cs` | `buttonPay_Click` |
| `frmSignIn.cs` | `buttonLogin_Click` |

---

## Plan de Corrección Priorizado

| Prioridad | # | Archivo | Problema | Acción | Impacto |
|---|---|---|---|---|---|
| **P0 — INMEDIATO** | G1 | `frmCustomerScreen.cs` ~L146 | `using var hFont = AppTypography.ColumnHeader` | Quitar `using` | Corrupción visual de headers |
| **P0 — INMEDIATO** | G2 | `CartListViewControl.cs` ~L78 | `using var headerFont = AppTypography.ColumnHeader` | Quitar `using` | Corrupción visual de headers |
| **P0 — INMEDIATO** | G3 | `UserSessionControl.cs` ~L219 | `using var inicialFont = AppTypography.AppHeader` | Quitar `using` | Avatar cashier en blanco |
| **P0 — INMEDIATO** | G4 | `UserSessionControl.cs` ~L228 | `using var nameFont = AppTypography.BodySmall` | Quitar `using` | Nombre cashier en blanco |
| **P1 — ESTA SEMANA** | A7 | `frmCierreDiario.cs` L.108 | `async void` cierre diario sin try/catch | Agregar try/catch con mensaje informativo | Crash en cierre de caja |
| **P1 — ESTA SEMANA** | A8 | `frmPopupCashPayment.cs` L.48 | `async void` impresión post-venta sin try/catch | Agregar try/catch | Crash post-venta |
| **P1 — ESTA SEMANA** | A3-6 | `frmPrintInvoice.cs` | 4 métodos `async void` sin protección | Agregar try/catch en cada uno | Crash en reimprimir recibo |
| **P1 — ESTA SEMANA** | A10 | `frmGiftCard.cs` L.94 | TextChanged sin try/catch (alta frecuencia) | Agregar try/catch silencioso | Crash por keystroke |
| **P1 — ESTA SEMANA** | A15 | `AutocompleteProductUserControl.cs` L.21 | TextChanged sin try/catch (alta frecuencia) | Agregar try/catch silencioso | Crash por keystroke |
| **P1 — ESTA SEMANA** | A13 | `frmSplit.cs` L.454 | EBT balance sin try/catch | Agregar try/catch | Crash en pago EBT |
| **P1 — ESTA SEMANA** | A9 | `frmSetting.cs` L.20 | Guardar config sin try/catch | Agregar try/catch | Crash al guardar |
| **P2 — PRÓXIMO SPRINT** | A1-2 | `frmProductNoExist.cs` | Load + Save sin try/catch | Agregar try/catch | Crash al crear producto |
| **P2 — PRÓXIMO SPRINT** | A14 | `frmCustomerScreen.cs` L.270 | `LoadCart()` sin try/catch | Agregar try/catch silencioso | Pantalla cliente sin carrito |
| **P2 — PRÓXIMO SPRINT** | A11-12 | `frmKeyLookup.cs`, `frmProductNew.cs` | Botones OK sin try/catch | Agregar try/catch | Crash en búsqueda/creación |
| **P2 — PRÓXIMO SPRINT** | G5 | `frmHome.cs` L.167 | ToolTip sin Dispose | Agregar a `components` | Handle HWND no liberado |
| **P3 — BACKLOG** | M2 | `UserSessionControl.cs` L.39-40 | Lambdas anónimas en `_ownerForm.MouseClick` | Convertir a métodos nombrados + override Dispose | Referencia pendiente post-dispose |
| **P3 — BACKLOG** | M3 | `frmHold.cs` L.29 | Service Locator en constructor | Migrar a constructor DI | Arquitectura |
| **P3 — BACKLOG** | G6 | `OmadaposLogo.cs` L.34 | Font creada en cada OnPaint | Cachear como campo estático | Presión GDI innecesaria |

---

## Impacto en Rendimiento y Estabilidad

| Área | Impacto | Descripción |
|---|---|---|
| **Estabilidad** | 🔴 CRÍTICO | 15 rutas de código producen crasheos de aplicación en condiciones de red/impresora que son normales en producción |
| **GDI integridad** | 🔴 CRÍTICO | 4 bugs destruyen fonts compartidas del Design System en el primer repintado → texto invisible en header |
| **Memoria** | 🟡 BAJO | Los leaks de eventos identificados son de bajo riesgo real (misma lifetime); el ToolTip es el más concreto |
| **Rendimiento** | 🟡 BAJO | Font en `OmadaposLogo.OnPaint` genera presión GDI innecesaria pero no es un leak |
