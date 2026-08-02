using System;
using UnityEngine;
using HutongGames.PlayMaker;
using MSCLoader;

namespace CarList
{
    /// <summary>
    /// Handles finding and caching references to the Sorbet car and its components.
    /// </summary>
    public class CarFinder
    {
        // Car references
        private GameObject _sorbetCar;
        private Rigidbody _mainCarBody;
        private Component _drivetrain;
        private Component _axles;
        private Component _carDynamics;
        private Component _axisController;

        // State flags
        private bool _carFound;
        private bool _drivetrainAnalyzed;
        private bool _axlesAnalyzed;

        // Throttling for fallback search (expensive operation)
        private float _lastFallbackSearchTime = -10f;
        private const float FallbackSearchCooldown = 1.0f; // Only search once per second

        // Original values (for reset)
        private float _originalMass;
        private float _originalMaxTorque;
        private float _originalMaxPower;
        private float _originalMaxRPM;
        private float[] _originalGearRatios;
        private float _originalFinalDrive;
        private Vector3 _originalCenterOfMass;
        private bool _comStored;

        // Public accessors
        public bool IsCarFound => _carFound && _mainCarBody != null;
        public GameObject Car => _sorbetCar;
        public Rigidbody CarBody => _mainCarBody;
        public Component Drivetrain => _drivetrain;
        public Component Axles => _axles;
        public Component CarDynamics => _carDynamics;
        public Component AxisController => _axisController;

        public float OriginalMass => _originalMass;
        public float OriginalMaxTorque => _originalMaxTorque;
        public float OriginalMaxPower => _originalMaxPower;
        public float OriginalMaxRPM => _originalMaxRPM;
        public float[] OriginalGearRatios => _originalGearRatios;
        public float OriginalFinalDrive => _originalFinalDrive;
        public Vector3 OriginalCenterOfMass => _originalCenterOfMass;
        public bool CenterOfMassStored => _comStored;

        public void StoreCenterOfMass()
        {
            if (_mainCarBody != null && !_comStored)
            {
                _originalCenterOfMass = _mainCarBody.centerOfMass;
                _comStored = true;
            }
        }

        /// <summary>
        /// Gets a display name for the car status.
        /// </summary>
        public string GetCarName()
        {
            if (_mainCarBody == null) return "Not Found";
            string driveStatus = _drivetrain != null ? " ✓" : " ⚠";
            return $"SORBET ({_mainCarBody.mass:F0} kg){driveStatus}";
        }

        /// <summary>
        /// Attempts to find the Sorbet car in the scene.
        /// </summary>
        public void FindSorbetCar(GameObject root = null)
        {
            if (root == null)
            {
                if (_carFound && _sorbetCar != null && _mainCarBody != null) return;

                // Search for car by name pattern (fast)
                root = GameObject.Find("SORBET(190-200psi)");

                if (root == null)
                {
                    // Throttle the expensive fallback search
                    if (Time.time - _lastFallbackSearchTime < FallbackSearchCooldown)
                    {
                        return; // Skip this frame, try again later
                    }
                    _lastFallbackSearchTime = Time.time;

                    // Fallback: search all Rigidbodies instead of all GameObjects (much faster)
                    foreach (Rigidbody rb in UnityEngine.Object.FindObjectsOfType<Rigidbody>())
                    {
                        if (rb.mass > 500f && rb.gameObject.name.Contains("SORBET") && rb.gameObject.name.Contains("psi"))
                        {
                            root = rb.gameObject;
                            ModConsole.Print($"Found Sorbet via fallback search: {root.name}");
                            break;
                        }
                    }
                }
            }

            if (root == null) return;

            _sorbetCar = root;
            _mainCarBody = root.GetComponent<Rigidbody>();

            if (_mainCarBody != null)
            {
                _originalMass = _mainCarBody.mass;
                _carFound = true;

                ModConsole.Print($"Found Sorbet main body: {root.name}");

                // Find all components
                FindDrivetrain();
                FindAxisController();
                FindCarDynamics();
            }
        }

        /// <summary>
        /// Clears all references and re-searches for the car.
        /// </summary>
        public void Refresh()
        {
            _carFound = false;
            _sorbetCar = null;
            _mainCarBody = null;
            _drivetrain = null;
            _axles = null;
            _carDynamics = null;
            _axisController = null;
            _drivetrainAnalyzed = false;
            _axlesAnalyzed = false;
            _comStored = false;

            FindSorbetCar();
        }

        private void FindDrivetrain()
        {
            if (_sorbetCar == null) return;

            foreach (Component c in _sorbetCar.GetComponents<Component>())
            {
                string typeName = c.GetType().Name;

                if (typeName == "Drivetrain")
                {
                    _drivetrain = c;
                    ModConsole.Print("Found Drivetrain component!");
                    AnalyzeDrivetrain();
                }
                else if (typeName == "Axles")
                {
                    _axles = c;
                    ModConsole.Print("Found Axles component!");
                }
            }

            if (_drivetrain == null)
            {
                ModConsole.Print("Drivetrain component not found!");
            }
        }

        private void FindAxisController()
        {
            if (_sorbetCar == null) return;

            foreach (Component c in _sorbetCar.GetComponents<Component>())
            {
                if (c.GetType().FullName == "AxisCarController")
                {
                    _axisController = c;
                    break;
                }
            }
        }

        private void FindCarDynamics()
        {
            if (_sorbetCar == null) return;

            foreach (Component c in _sorbetCar.GetComponents<Component>())
            {
                if (c.GetType().FullName == "CarDynamics")
                {
                    _carDynamics = c;
                    break;
                }
            }
        }

        private void AnalyzeDrivetrain()
        {
            if (_drivetrain == null || _drivetrainAnalyzed) return;

            try
            {
                // Get gear ratios
                /* TODO
                 
                var gearField = ReflectionCache.GetField(_drivetrain.GetType(), TuningConstants.FieldNames.GearRatios);
                if (gearField != null)
                {
                    float[] ratios = gearField.GetValue(_drivetrain) as float[];
                    if (ratios != null)
                    {
                        _originalGearRatios = (float[])ratios.Clone();
                    }
                }*/

                _drivetrainAnalyzed = true;
                ModConsole.Print("Drivetrain analyzed successfully.");
            }
            catch (Exception ex)
            {
                ModConsole.Print($"Failed to analyze drivetrain: {ex.Message}");
            }
        }

        /// <summary>
        /// Performs periodic analysis if not yet completed.
        /// Call from Update() loop.
        /// </summary>
        public void PeriodicAnalysis()
        {
            if (!_axlesAnalyzed && _axles != null)
            {
                _axlesAnalyzed = true; // Just mark as done, detailed analysis not needed for tuning
            }
            if (!_drivetrainAnalyzed && _drivetrain != null)
            {
                AnalyzeDrivetrain();
            }
        }
    }
}