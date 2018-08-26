﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public sealed class HVACExhaustChannel : AirChannel, IDynamicObject, IResetableObject
    {
        public HVACExhaustChannel() :base()
        {
            //InputAir = new Air(20, 40, EAirHum.relative);
            HVACObjectsList.Add(new HVACFilter(true));
            HVACObjectsList.Add(new HVACFan(true));
            HVACObjectsList.Add(new HVACMixingBox(false));
            HVACObjectsList.Add(new HVACOutletExchange());
            SubscribeToAllItems();
            Name = "Kanał wywiewny";
            InitializePlotDataList();

            SetInitialValuesParameters();
            ResetableObjects.AddRange(HVACObjectsList);
        }

        public void UpdateParams()
        {
            foreach (HVACObject obj in HVACObjectsList)
            {
                if (obj is IDynamicObject)
                {
                    ((IDynamicObject)obj).UpdateParams();
                }
            }
        }

        public HVACOutletExchange GetOutletExchange()
        {
            foreach (HVACObject obj in HVACObjectsList)
            {
                if (obj is HVACOutletExchange) return (HVACOutletExchange)obj;
            }
            throw new Exception("Brak wylotu powietrza wymiennika w kanale wywiewnym");
        }

        protected override void InitializePlotDataList()
        {
            
        }

        public override void SetInitialValuesParameters()
        {
            base.SetInitialValuesParameters();
        }
    }
}
