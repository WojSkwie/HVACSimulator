﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public enum EState
    {
        running,
        paused,
        stopped,
    }

    public sealed class GlobalParameters : IResetableObject
    {
        private static GlobalParameters _Instance;
        private GlobalParameters()
        {
            SimulationState = EState.stopped;
            ResetTime();
        }
        public static GlobalParameters Instance
        {
            get
            {
                if (_Instance == null) _Instance = new GlobalParameters();
                return _Instance;
            }
        }

        public double SimulationTime { get; private set; }
        public EState SimulationState { get; set; }
        public EventHandler<string> SimulationErrorOccured;

        public void IncrementTime()
        {
            SimulationTime += Constants.step;
        }

        public void ResetTime()
        {
            SimulationTime = 0;
        }

        public void OnErrorSimulationOccured(object sender, string error)
        {
            SimulationErrorOccured?.Invoke(this, error);
        }

        public void SetInitialValuesParameters()
        {
            ResetTime();
        }
    }
}
