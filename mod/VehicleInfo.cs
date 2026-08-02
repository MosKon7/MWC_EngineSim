using System.Collections.Generic;
using UnityEngine;

namespace CarList
{
    /// Простая инфа по одному транспорту.
    public class VehicleInfo
    {
        public GameObject Root;
        public Rigidbody Body;
        public Component Drivetrain;
        public Component Axles;
        public Component CarDynamics;
        public Component AxisController;

        // Оригинальные значения (по минимуму, можно расширить)
        public float OriginalMass;
        public Vector3 OriginalCenterOfMass;
        public bool CenterOfMassStored;
    }

    /// Поиск всех транспортных средств в сцене.
    public class VehicleFinder
    {
        // Найденные машины
        private readonly List<VehicleInfo> _vehicles = new List<VehicleInfo>();
        private readonly Dictionary<string, VehicleInfo> _byName = new Dictionary<string, VehicleInfo>();

        public VehicleInfo Gifu { get; private set; }
        public VehicleInfo Sorbet { get; private set; }
        public VehicleInfo Bus { get; private set; }
        public VehicleInfo Kekmet { get; private set; }
        public VehicleInfo Heppa { get; private set; }
        public VehicleInfo Bachglotz { get; private set; }
        public VehicleInfo Policecar1 { get; private set; }
        public VehicleInfo Policecar2 { get; private set; }

        // Настройки поиска
        private const float MinVehicleMass = 300f;          // отсечь мелкие объекты
        private const float ScanCooldown = 1.0f;            // раз в секунду
        private float _lastScanTime = -10f;

        public List<VehicleInfo> Vehicles
        {
            get { return _vehicles; }
        }

        public void InitializeOnce()
        {
            _vehicles.Clear();
            _byName.Clear();
            ScanForVehiclesInternal();
            BuildNamedIndex();
        }

        /// <summary>
        /// Rebuilds the full vehicle list
        /// </summary>
        public void RefreshAll()
        {
            _vehicles.Clear();
            _byName.Clear();
            Gifu = null;
            Sorbet = null;
            Bus = null;
            Kekmet = null;
            Heppa = null;
            Bachglotz = null;
            Policecar1 = null;
            Policecar2 = null;
            ScanForVehiclesInternal(force: true);
            BuildNamedIndex();
        }

        /// Вызывать из Update() мода.
        public void PeriodicUpdate()
        {
            ScanForVehiclesInternal();
        }

        /// Поиск всех «машин» через Rigidbody + компоненты.
        private void ScanForVehiclesInternal(bool force = false)
        {
            foreach (Rigidbody rb in UnityEngine.Object.FindObjectsOfType<Rigidbody>())
            {
                if (rb == null || rb.mass < 300f)
                    continue;

                GameObject go = rb.gameObject;

                Component drivetrain = FindComponentByName(go, "Drivetrain");
                Component carDyn = FindComponentByName(go, "CarDynamics");
                Component axles = FindComponentByName(go, "Axles");
                Component axisCtrl = FindComponentByName(go, "AxisCarController");

                if (drivetrain == null && carDyn == null && axles == null && axisCtrl == null)
                    continue;

                var info = new VehicleInfo
                {
                    Root = go,
                    Body = rb,
                    Drivetrain = drivetrain,
                    Axles = axles,
                    CarDynamics = carDyn,
                    AxisController = axisCtrl,
                    OriginalMass = rb.mass,
                    OriginalCenterOfMass = rb.centerOfMass,
                    CenterOfMassStored = true
                };

                _vehicles.Add(info);
            }
        }

        /// Универсальный поиск компонента по имени типа (без прямой ссылки на сборку).
        private Component FindComponentByName(GameObject obj, string typeName)
        {
            foreach (Component c in obj.GetComponents<Component>())
            {
                if (c == null) continue;
                if (c.GetType().Name == typeName || c.GetType().FullName.EndsWith("." + typeName))
                    return c;
            }
            return null;
        }

        /// Получить «человекочитаемое» имя машины.
        public string GetVehicleDisplayName(VehicleInfo v)
        {
            if (v == null || v.Body == null)
                return "Not Found";

            string driveStatus = v.Drivetrain != null ? "✓" : "⚠";
            return $"{v.Root.name} ({v.Body.mass:F0} kg) {driveStatus}";
        }
        private void BuildNamedIndex()
        {
            foreach (var v in _vehicles)
            {
                string name = v.Root.name;

                _byName[name] = v;

                if (name.StartsWith("GIFU"))
                    Gifu = v;
                else if (name.StartsWith("SORBET"))
                    Sorbet = v;
                else if (name.StartsWith("BUS"))
                    Bus = v;
                else if (name.StartsWith("KEKMET"))
                    Kekmet = v;
                else if (name.StartsWith("HEPPA"))
                    Heppa = v;
                else if (name.StartsWith("BACHGLOTZ"))
                    Bachglotz = v;
                else if (name.StartsWith("POLICECAR1"))
                    Policecar1 = v;
                else if (name.StartsWith("POLICECAR2"))
                    Policecar2 = v;
            }
        }

        public VehicleInfo GetByExactName(string name)
        {
            VehicleInfo v;
            return _byName.TryGetValue(name, out v) ? v : null;
        }
    }
}
