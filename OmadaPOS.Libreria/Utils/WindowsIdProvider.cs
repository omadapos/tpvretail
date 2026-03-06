using System.Management;

namespace OmadaPOS.Libreria.Utils
{
    public static class WindowsIdProvider
    {
        public static string? GetMachineGuid()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT UUID FROM Win32_ComputerSystemProduct");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    return queryObj["UUID"]?.ToString();
                }
            }
            catch
            {
                // Manejo de errores según sea necesario
            }
            return null;
        }
    }
}