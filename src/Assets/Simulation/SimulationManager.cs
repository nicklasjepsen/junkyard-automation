using System;
using UnityEngine;

namespace JunkyardAutomation.Simulation
{
    /// <summary>
    /// Manages the deterministic tick-based simulation loop.
    /// Runs at a fixed timestep independent of frame rate.
    /// </summary>
    public class SimulationManager : MonoBehaviour
    {
        public static SimulationManager Instance { get; private set; }

        [Header("Simulation Settings")]
        [SerializeField] private int ticksPerSecond = 20;
        [SerializeField] private bool startPaused = false;

        private bool isPaused;
        private long currentTick;
        private float tickAccumulator;
        private float tickInterval;

        public bool IsPaused
        {
            get => isPaused;
            set
            {
                if (isPaused != value)
                {
                    isPaused = value;
                    Debug.Log($"[SimulationManager] {(isPaused ? "Paused" : "Resumed")} at tick {currentTick}");
                }
            }
        }

        public long CurrentTick => currentTick;
        public int TicksPerSecond => ticksPerSecond;

        /// <summary>
        /// Fired after each simulation tick.
        /// </summary>
        public event Action<long> OnTick;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            tickInterval = 1f / ticksPerSecond;
            isPaused = startPaused;
        }

        private void Start()
        {
            Debug.Log($"[SimulationManager] Started - {ticksPerSecond} ticks/second ({tickInterval * 1000:F1}ms per tick)");
        }

        private void Update()
        {
            // Handle pause toggle
            if (UnityEngine.InputSystem.Keyboard.current != null &&
                UnityEngine.InputSystem.Keyboard.current.pKey.wasPressedThisFrame)
            {
                IsPaused = !IsPaused;
            }

            if (isPaused) return;

            // Accumulate time and run ticks
            tickAccumulator += Time.deltaTime;

            // Cap accumulated time to prevent spiral of death
            if (tickAccumulator > tickInterval * 10)
            {
                tickAccumulator = tickInterval * 10;
            }

            while (tickAccumulator >= tickInterval)
            {
                tickAccumulator -= tickInterval;
                ExecuteTick();
            }
        }

        private void ExecuteTick()
        {
            currentTick++;

            // Get yard state from PlacementManager
            var yard = JunkyardAutomation.Placement.PlacementManager.Instance?.YardState;
            if (yard == null) return;

            // Run systems in order
            ConveyorSystem.Tick(yard);

            // Fire event
            OnTick?.Invoke(currentTick);
        }

        /// <summary>
        /// Get interpolation factor for smooth rendering between ticks.
        /// Returns 0-1 value representing progress to next tick.
        /// </summary>
        public float GetTickInterpolation()
        {
            return tickAccumulator / tickInterval;
        }
    }
}
