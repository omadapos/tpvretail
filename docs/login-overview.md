# Login — frmSignIn

**Archivo:** `OmadaPOS/Views/frmSignIn.cs` + `frmSignIn.Designer.cs`  
**Pantalla objetivo:** Elo 15" widescreen (~1366×768, touch)

---

## Flujo resumido

```
App.Run(frmSignIn)
  └── FrmSignIn_Load
        ├── Muestra terminal ID en footer (WindowsIdProvider.GetMachineGuid)
        ├── Inicia clock Timer (1 seg → lblClock)
        └── Centra pnlCard en pantalla

Usuario toca dígitos → AppendDigit(_pin += digit) → pnlPinDots.Invalidate()
Usuario toca ⌫     → borra último dígito de _pin
Usuario toca ▶     → ButtonLogin_Click → DoLoginAsync()

DoLoginAsync()
  ├── userService.Login(LoginRequest { Email=_pin, Password="12345678", WindowsId })
  ├── OK  → SessionManager ← Token/BranchId/Name/AdminId/Phone
  │         Hide() → _windowService.OpenHome()
  └── FAIL → ShowError(msg) + ClearPin()
```

---

## Layout

```
┌─ pnlHeader (56px, slate-900) ──────────────────────────────┐
│  ● Omada POS                              10:32 AM          │
│ ─────────────────────────────────────────── [emerald 2px] ──│
├─ pnlBackground (Fill, slate-900) ──────────────────────────┤
│                                                             │
│            ┌─ pnlCard (400×560, white) ─┐                  │
│            │ [emerald border 3px top]   │                   │
│            │                            │                   │
│            │  "Enter Employee PIN"      │  ← lblTitle       │
│            │  ● ● ● ● ─────────────    │  ← pnlPinDots     │
│            │  [error inline, oculto]    │  ← lblError       │
│            │                            │                   │
│            │   7    8    9              │                   │
│            │   4    5    6              │  ← tlpKeypad      │
│            │   1    2    3              │    3×4, Fill      │
│            │   ⌫    0    ▶             │                   │
│            └────────────────────────────┘                   │
│                                                             │
├─ pnlFooter (34px, slate-900) ──────────────────────────────┤
│         Terminal: xxxxxxxx-xxxx-xxxx-xxxx                   │
└─────────────────────────────────────────────────────────────┘
```

---

## Controles clave

| Control | Tipo | Propósito |
|---|---|---|
| `pnlHeader` | Panel, DockStyle.Top, 56px | Header con logo y reloj |
| `lblLogo` | Label | "● Omada POS" — Segoe UI 14pt Bold |
| `lblClock` | Label | Hora actual, actualiza cada segundo |
| `pnlBackground` | Panel, DockStyle.Fill | Fondo oscuro, centra la card |
| `pnlCard` | Panel, 400×560px | Card blanca centrada con borde emerald |
| `pnlPinDots` | Panel (custom paint) | Muestra círculos por cada dígito ingresado |
| `lblTitle` | Label | Instrucción "Enter Employee PIN" |
| `lblError` | Label | Error inline, auto-oculto 3.5 seg |
| `tlpKeypad` | TableLayoutPanel 3×4 | Grilla del teclado numérico |
| `button0–9` | Button (navy/white) | Dígitos, ~114×84px touch target |
| `buttonClear` | Button (amber) | Borra último dígito (⌫) |
| `buttonLogin` | Button (emerald) | Envía el login (▶) |
| `labelId` | Label | Terminal GUID en el footer |
| `pnlFooter` | Panel, DockStyle.Bottom, 34px | Pie de página con ID de terminal |

---

## Campos privados (frmSignIn.cs)

| Campo | Tipo | Descripción |
|---|---|---|
| `_pin` | `string` | Acumula los dígitos tecleados |
| `_clock` | `Timer` | Actualiza `lblClock` cada segundo |
| `_loginInProgress` | `bool` | Bloquea doble-submit |
| `_errorCts` | `CancellationTokenSource?` | Cancela el auto-hide del error anterior |

---

## Colores (tema Slate/Emerald)

| Elemento | Color |
|---|---|
| Fondo form/background | `#0F1729` (slate-900) |
| Card | `#FFFFFF` (blanco) |
| Borde superior card | `#059669` (emerald-600) |
| Botones numéricos | `#0F1729` texto blanco |
| Botón Clear | `#B45309` (amber-700) |
| Botón Login | `#059669` (emerald-600) |
| PIN dots rellenos | `#0F1729` (slate-900) |
| PIN dots vacíos | `#CBD5E1` (slate-300) |
| Reloj | `#34D399` (emerald-400) |
| Error text | `#DC2626` (red-600) |

---

## Métodos principales

### `AppendDigit(string digit)`
Agrega un dígito a `_pin` (máx 20) e invalida `pnlPinDots` para redibujar.  
Llamado desde los lambdas de `ConfigureNumButton()` en el Designer.

### `PnlPinDots_Paint`
Pinta 12 círculos de 13px. Los primeros `_pin.Length` se dibujan en navy, el resto en gris claro. Una línea emerald los subraya.

### `DoLoginAsync()`
```csharp
LoginRequest { Email = _pin, Password = "12345678", WindowsId = labelId.Text }
```
> **Nota:** La `Password` es una clave fija de sistema. Pendiente migrar a PIN individual por empleado en la API.

### `ShowError(string message)`
Muestra `lblError` y lo oculta automáticamente a los 3.5 segundos usando `CancellationTokenSource` para cancelar el hide anterior si el usuario reintenta.

### `CenterCard()`
Calcula `pnlCard.Location` en el centro de `pnlBackground`. Se llama en `Load` y en el evento `Resize` del background.

---

## Notas de diseño

- **Sin `TableLayoutPanel` de 3 columnas:** La versión anterior desperdiciaba el 61% del ancho con un panel de branding. Ahora el 100% del espacio es el keypad.
- **Sin `MessageBox`:** Los errores se muestran inline para no interrumpir el flujo táctil.
- **Sin TextBox visible:** El PIN vive en `_pin` (string privado). Nunca se renderiza en texto plano.
- **Fuente del sistema:** Segoe UI en lugar de Montserrat (no instalada por defecto), elimina el riesgo de fallback.
- **Touch targets:** ~114×84px por botón, supera el mínimo recomendado de 44×44px.
