/**
* AYController.cs
*
* AmpYear power management.
* (C) Copyright 2015, Jamie Leighton
* The original code and concept of AmpYear rights go to SodiumEyes on the Kerbal Space Program Forums, which was covered by GNU License GPL (no version stated).
* As such this code continues to be covered by GNU GPL license.
* Parts of this code were copied from Fusebox by the user ratzap on the Kerbal Space Program Forums, which is covered by GNU License GPLv2.
* Concepts which are common to the Game Kerbal Space Program for which there are common code interfaces as such some of those concepts used
* by this program were based on:
* Thunder Aerospace Corporation's Life Support for Kerbal Space Program.
* Written by Taranis Elsu.
* (C) Copyright 2013, Taranis Elsu
* Which is licensed under the Attribution-NonCommercial-ShareAlike 3.0 (CC BY-NC-SA 3.0)
* creative commons license. See <http://creativecommons.org/licenses/by-nc-sa/3.0/legalcode>
* for full details.
*
* Thanks go to both ratzap and Taranis Elsu for their code.
* Kerbal Space Program is Copyright (C) 2013 Squad. See http://kerbalspaceprogram.com/. This
* project is in no way associated with nor endorsed by Squad.
*
*  This file is part of AmpYear.
*
*  AmpYear is free software: you can redistribute it and/or modify
*  it under the terms of the GNU General Public License as published by
*  the Free Software Foundation, either version 3 of the License, or
*  (at your option) any later version.
*
*  AmpYear is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*  GNU General Public License for more details.
*
*  You should have received a copy of the GNU General Public License
*  along with AmpYear.  If not, see <http://www.gnu.org/licenses/>.
*
*/
using System;
using System.Collections.Generic;
using System.Linq;
using ModuleWheels;
using UnityEngine;
using RSTUtils;

namespace AY
{
    public partial class AYController : MonoBehaviour
    {

        //Temp vars for PartModule Processing
        //Stock
        private BaseFieldList fieldlist;
        private object state;
        private object ec_rate;
        private object speed;
        private object lamps;
        private object is_enabled;
        private object co2_rate;
        private ModuleRCS tmpRCSmodule;
        private ModuleDeployableSolarPanel tmpSol;
        private double solarFlux;
        private double orientationFactor;
        private float multiplier;
        private ModuleGenerator tmpGen;
        private ModuleActiveRadiator tmprad;
        private ModuleLight tmpLight;
        private ModuleDataTransmitter tmpAnt;
        private ModuleReactionWheel tmpRw;
        //private ReactionWheelPower rwp;
        private ModuleEngines tmpEng;
        private const float grav = 9.81f;
        private bool usesCharge = false;
        private float sumRd = 0;
        private Single ecratio = 0;
        private bool tmpcurrentEngActive = false;
        private ModuleEnginesFX tmpEngFx;
        private ModuleAlternator tmpAlt;
        private double tmpaltRate = 0f;
        private ModuleResourceConverter tmpRegRc;
        private ConversionRecipe recipe;
        private float FillAmount;
        private List<ResourceRatio> recInputs;
        private List<ResourceRatio> recOutputs;
        private ModuleEnviroSensor tmpEnvS;

        //Other Mods
        private ALWrapper.ALNavLight ALtmpLight;
        private string generatorState;
        private string[] generatorStatewords;
        private int stringlength;
        private NFSWrapper.NFSCurvedPanel NFSCPtmpGen;
        private KASWrapper.KASModuleWinch tmpKw;
        private KASWrapper.KASModuleMagnet tmpKm;
        private RTWrapper.RTAntenna RTtmpAnt;
        private ScanSatWrapper.SCANsat tmpSs;
        private TeleWrapper.TMPowerDrain tmpTm;
        private TACLSWrapper.TACLSGenericConverter tacGC;
        private string resName;
        private KSPIEWrapper.FNModuleCryostat tmpCryo;
        private KSPIEWrapper.FNGenerator FNtmpGen;
        private DFWrapper.DeepFreezer tmpDeepFreezer;
        private KPBSWrapper.ModuleKPBSConverter KPBSconv;
        private KPBSWrapper.PlanetaryGreenhouse KPBSgh;
        private float currentRateConverter;
        private float currentRateGreenhouse;
        private USILSWrapper.ModuleLifeSupportSystem usiMLS;
        private USILSWrapper.ModuleSwappableConverter usiMSC;
        private IONRCSWrapper.ModuleIONPoweredRCS tmpIonPoweredRcs;
        private IONRCSWrapper.ModulePPTPoweredRCS tmpPPTPoweredRcs;
        private float IONRCSelecUse;
        private NFPWrapper.VariableISPEngine tmpVariableIspEngine;


        #region StockPartModules

        internal bool ProcessStockPartModule(Part currentPart, PartModule module, bool inhasAlternator, bool incurrentEngActive, double inaltRate,
            out bool hasAlternator, out bool currentEngActive, out double altRate)
        {
            currentEngActive = false;
            hasAlternator = false;
            altRate = 0;
            prtName = currentPart.name;

            if (module is LaunchClamp)
                return true; // skip clamps

            if (module is ModuleAlternator)
                hasAlternator = true;

            if (module is ModuleRCS)
            {
                ProcessModuleRCS(prtName, currentPart, module);
                return true;
            }

            if (module.moduleName == "ModuleDeployableSolarPanel")
            {
                ProcessModuleDeployableSolarPanel(prtName, currentPart, module);
                return true;
            }

            if (module.moduleName == "ModuleGenerator")
            {
                ProcessModuleGenerator(prtName, currentPart, module);
                return true;
            }

            if (module.moduleName == "ModuleWheelMotor")
            {
                ProcessModuleWheelMotor(prtName, currentPart, module);
                return true;
            }

            if (module.moduleName == "ModuleCommand")
            {
                ProcessModuleCommand(prtName, currentPart, module);
                return true;
            }

            if (module.moduleName == "ModuleLight")
            {
                ProcessModuleLight(prtName, currentPart, module);
                return true;
            }

            if (module.moduleName == "ModuleDataTransmitter")
            {
                ProcessModuleDataTransmitter(prtName, currentPart, module);
                return true;
            }

            if (module.moduleName == "ModuleReactionWheel")
            {
                ProcessModuleReactionWheel(prtName, currentPart, module);
                return true;
            }

            if (module.moduleName == "ModuleEngines")
            {
                currentEngActive = ProcessModuleEngines(prtName, currentPart, module, inaltRate);
                return true;
            }

            if (module.moduleName == "ModuleEnginesFX")
            {
                currentEngActive = ProcessModuleEnginesFX(prtName, currentPart, module, inaltRate);
                return true;
            }

            if (module.moduleName == "ModuleAlternator")
            {
                altRate = ProcessModuleAlternator(prtName, currentPart, module, incurrentEngActive);
                return true;
            }

            if (module.moduleName == "ModuleScienceLab")
            {
                ProcessModuleScienceLab(prtName, currentPart, module);
                return true;
            }

            if (module.moduleName == "ModuleScienceConverter")
            {
                ProcessModuleScienceConverter(prtName, currentPart, module);
                return true;
            }

            if (module.moduleName == "ModuleResourceHarvester")
            {

                ProcessModuleResourceharvester(prtName, currentPart, module);
                return true;
            }

            if (module.moduleName == "ModuleResourceConverter")
            {
                ProcessModuleResourceConverter(prtName, currentPart, module, 0);
                return true;
            }

            if(module.moduleName == "ModuleActiveRadiator")
            {
                ProcessModuleActiveRadiator(prtName, currentPart, module);
                return true;
            }

            if (module.moduleName == "ModuleEnviroSensor")
            {
                ProcessModuleEnviroSensor(prtName, currentPart, module);
                return true;
            }

            return false;
        }
        
        private void ProcessModuleRCS(string prtName, Part currentPart, PartModule module)
        {
            prtActive = false;
            prtPower = "";
            tmpPower = 0f;
            HasRcs = true;

            tmpRCSmodule = (ModuleRCS)module;
            for (int i = tmpRCSmodule.thrustForces.Length - 1; i >= 0; --i)
            {
                currentRCSThrust += tmpRCSmodule.thrustForces[i];
            }

            if (IONRCSPresent)
            {
                checkIONRCS(module, currentPart);
            }
        }
        
        private void ProcessModuleDeployableSolarPanel(string prtName, Part currentPart, PartModule module)
        {
            prtActive = false;
            prtPower = "";
            tmpPower = 0f;
            tmpSol = (ModuleDeployableSolarPanel)module;
            if (Utilities.GameModeisFlight)
            {
                //prtActive = tmpSol.panelState == ModuleDeployableSolarPanel.panelStates.EXTENDED ||
                //        tmpSol.panelState == ModuleDeployableSolarPanel.panelStates.EXTENDING;
                prtActive = tmpSol.deployState == ModuleDeployablePart.DeployState.EXTENDED ||
                            tmpSol.deployState == ModuleDeployablePart.DeployState.EXTENDING;
                tmpPower = tmpSol.flowRate;
                //Utilities.Log_Debug("totalPowerProduced SolarPanel Power = " + tmpPower + " Part = " + currentPart.name);
            }
            else
            {
                prtActive = true;
                //tmpPower = tmpSol.chargeRate;
                
                //Get the distance and direction to Sun from the currently selected target body
                Utilities.CelestialBodyDistancetoSun(FlightGlobals.Bodies[_selectedDarkTarget],out sun_dir,out sun_dist);
                solarFlux = Utilities.SolarLuminosity / (12.566370614359172 * sun_dist * sun_dist);
                orientationFactor = 1;  //Just use one.
                multiplier = 1;
                if (tmpSol.useCurve)
                {
                    multiplier = tmpSol.powerCurve.Evaluate((float) sun_dist);
                }
                else
                {
                    multiplier = (float)(solarFlux / PhysicsGlobals.SolarLuminosityAtHome);
                }
                tmpPower = tmpSol.chargeRate * orientationFactor * multiplier;  //Does not take into account temperature curve
            }
            AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, module.moduleName, false, prtActive, tmpPower, true, true);
            ProcessPartEmergencyShutdownProcedures(currentPart, module, prtActive);
        }
        
        private void ProcessModuleGenerator(string prtName, Part currentPart, PartModule module)
        {
            prtActive = false;
            prtPower = "";
            tmpPower = 0f;
            tmpGen = (ModuleGenerator)module;
            double ECConsumed = 0f;
            double ECProduced = 0f;
            ProcessResHandler("Both", module, out ECConsumed, out ECProduced);
            
            if (Utilities.GameModeisEditor)
            {
                tmpPower = ECProduced;
                prtActive = true;
            }
            else
            {
                if (tmpGen.isAlwaysActive || tmpGen.generatorIsActive)
                {
                    tmpPower = ECProduced;
                    //Utilities.Log_Debug("totalPowerProduced Generator Output Active Power = " + tmpPower + " Part = " + currentPart.name);
                    prtActive = true;
                }
                else
                {
                    tmpPower = ECProduced;
                    prtActive = false;
                }
            }
            AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, module.moduleName, false, prtActive, tmpPower, true, false);
                
            
            if (Utilities.GameModeisEditor)
            {
                tmpPower = ECConsumed;
                prtActive = true;
            }
            else
            {
                if (tmpGen.isAlwaysActive || tmpGen.generatorIsActive)
                {
                    tmpPower = ECConsumed;
                    prtActive = true;
                }
                else
                {
                    tmpPower = ECConsumed;
                    prtActive = false;
                }
            }
            AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, module.moduleName, false, prtActive, tmpPower, false, false);
                
        }

        private void ProcessModuleWheelMotor(string prtName, Part currentPart, PartModule module)
        {
            prtActive = false;
            prtPower = "";
            tmpPower = 0f;
            ModuleWheelMotor tmpWheel = (ModuleWheelMotor)module;
            //if (tmpWheel.resHandler.inputResources .inputResource.name == MAIN_POWER_NAME)
            //{
                if (Utilities.GameModeisFlight)
                {
                    //tmpPower = (float)tmpWheel.inputResource.currentAmount;
                    if (tmpWheel.state == ModuleWheelMotor.MotorState.Idle || tmpWheel.state == ModuleWheelMotor.MotorState.Running)
                    {
                        prtActive = true;
                    }
                }

                if (Utilities.GameModeisEditor)
                {
                    //tmpPower = tmpWheel.inputResource.rate + tmpWheel.idleDrain;
                    prtActive = true;
                }
                if (prtActive)
                {
                    double ECConsumed = 0f;
                    double ECProduced = 0f;
                    ProcessResHandler("Input", module, out ECConsumed, out ECProduced);
                    tmpPower = ECConsumed;
                    if (Utilities.GameModeisEditor)
                    {
                        tmpPower += tmpWheel.idleDrain;
                    }
                }
                AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, module.moduleName, false, prtActive, tmpPower, false, false);
                ProcessPartEmergencyShutdownProcedures(currentPart, module, prtActive);
            //}
        }

        private void ProcessModuleCommand(string prtName, Part currentPart, PartModule module)
        {
            prtActive = false;
            prtPower = "";
            tmpPower = 0f;
            if (TACLPresent)
                try
                {
                    checkTACL(module, currentPart, true);
                }
                catch
                {
                    Utilities.Log("Wrong TAC LS library version - disabled.");
                    TACLPresent = false;
                }

            ModuleCommand tmpPod = (ModuleCommand)module;
            double ECConsumed = 0f;
            double ECProduced = 0f;
            ProcessResHandler("Input", module, out ECConsumed, out ECProduced);
            tmpPower = ECConsumed;
            
            if (Utilities.GameModeisEditor)
            {
                prtActive = true;
                tmpPower = ECConsumed;
            }
            else
            {
                prtActive = ECConsumed > 0;
                tmpPower = ECConsumed;
            }
            if (prtActive)
                AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, module.moduleName, false, prtActive, tmpPower, false, false);
            PartsModuleCommand.Add(currentPart);
        }
        
        private void ProcessModuleActiveRadiator(string prtName, Part currentPart, PartModule module)
        {
            prtActive = false;
            prtPower = "";
            tmpPower = 0f;
            tmprad = (ModuleActiveRadiator)module;
            if (Utilities.GameModeisEditor || (Utilities.GameModeisFlight && tmprad.IsCooling))
            {
                prtActive = true;
                double ECConsumed = 0f;
                double ECProduced = 0f;
                ProcessResHandler("Input", module, out ECConsumed, out ECProduced);
                tmpPower = ECConsumed;
            }
            AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, module.moduleName, false, prtActive, tmpPower, false, false);
            ProcessPartEmergencyShutdownProcedures(currentPart, module, prtActive);
        }
        
        private void ProcessModuleLight(string prtName, Part currentPart, PartModule module)
        {
            prtActive = false;
            prtPower = "";
            tmpPower = 0f;
            tmpLight = (ModuleLight)module;
            if (Utilities.GameModeisEditor || (Utilities.GameModeisFlight && tmpLight.isOn))
            {
                prtActive = true;
                tmpPower = tmpLight.resourceAmount;
            }
            AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, module.moduleName, false, prtActive, tmpPower, false, false);
            ProcessPartEmergencyShutdownProcedures(currentPart, module, prtActive);
        }
        
        private void ProcessModuleDataTransmitter(string prtName, Part currentPart, PartModule module)
        {
            prtActive = false;
            prtPower = "";
            tmpPower = 0f;
            tmpAnt = (ModuleDataTransmitter)module;

            if (Utilities.GameModeisEditor || (Utilities.GameModeisFlight && tmpAnt.IsBusy()))
            {
                tmpPower = tmpAnt.DataResourceCost * (1 / tmpAnt.packetInterval);
                prtActive = true;
            }
            else
            {
                tmpPower = 0;
                prtActive = false;
            }
            AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, module.moduleName, false, prtActive, tmpPower, false, false);
            ProcessPartEmergencyShutdownProcedures(currentPart, module, prtActive);
        }
        
        private void ProcessModuleReactionWheel(string prtName, Part currentPart, PartModule module)
        {
            prtActive = false;
            prtPower = "";
            tmpPower = 0f;
            tmpRw = (ModuleReactionWheel)module;
            prtActive = tmpRw.enabled;

            tmpPower = 0;
            
            if (prtActive)
            {
                double ECConsumed = 0f;
                double ECProduced = 0f;
                ProcessResHandler("Input", module, out ECConsumed, out ECProduced);
                if (Utilities.GameModeisEditor)
                {
                    //tmpPower += tmpRw.inputResources[i].rate * tmpRw.PitchTorque; // rough guess for VAB
                    tmpPower = ECConsumed*tmpRw.PitchTorque; // rough guess for VAB
                }
                else
                {
                    //tmpPower += tmpRw.inputResources[i].currentAmount;
                    tmpPower = ECConsumed;
                    _sasPwrDrain += ECConsumed;// tmpRw.inputResources[i].currentAmount;
                }
            }
            
            AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, module.moduleName, false, prtActive, tmpPower, false, false);
            ProcessPartEmergencyShutdownProcedures(currentPart, module, prtActive);
        }
        
        private bool ProcessModuleEngines(string prtName, Part currentPart, PartModule module, double altRate)
        {
            prtActive = false;
            prtPower = "";
            tmpPower = 0f;
            sumRd = 0f;
            tmpEng = (ModuleEngines)module;
            for (int i = tmpEng.propellants.Count - 1; i >= 0; --i)
            {
                if (tmpEng.propellants[i].name == MAIN_POWER_NAME)
                {
                    usesCharge = true;
                    ecratio = tmpEng.propellants[i].ratio;
                }
                sumRd += tmpEng.propellants[i].ratio * PartResourceLibrary.Instance.GetDefinition(tmpEng.propellants[i].id).density;
            }
            if (usesCharge)
            {
                float massFlowRate = 0;
                if (Utilities.GameModeisFlight && tmpEng.isOperational && tmpEng.currentThrottle > 0)
                {
                    prtActive = true;
                    massFlowRate = tmpEng.currentThrottle * tmpEng.maxThrust / (tmpEng.atmosphereCurve.Evaluate(0) * grav);

                    tmpPower = ecratio * massFlowRate / sumRd;
                }
                if (Utilities.GameModeisEditor)
                {
                    prtActive = true;
                    massFlowRate = 1.0f * tmpEng.maxThrust / (tmpEng.atmosphereCurve.Evaluate(0) * grav);

                    tmpPower = ecratio * massFlowRate / sumRd;
                }
                AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, module.moduleName, false, prtActive, tmpPower, false, false);
            }

            tmpcurrentEngActive = tmpEng.isOperational && (tmpEng.currentThrottle > 0);
            if (altRate > 0 && tmpcurrentEngActive)
            {
                //Utilities.Log_Debug("totalPowerProduced ModEngine Active Power = " + altRate + " Part = " + currentPart.name);

                prtActive = true;
                AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, module.moduleName, false, prtActive, (float)altRate, true, false);

            }
            return tmpcurrentEngActive;
        }
      
        private bool ProcessModuleEnginesFX(string prtName, Part currentPart, PartModule module ,double altRate)
        {
            prtActive = false;
            prtPower = "";
            tmpPower = 0f;
            tmpEngFx = (ModuleEnginesFX)module;
            sumRd = 0f;
            //const float grav = 9.81f;
            //bool usesCharge = false;
            //float sumRd = 0;
            //Single ecratio = 0;
            //bool currentEngActive = false;
            for (int i = tmpEngFx.propellants.Count - 1; i >= 0; --i)
            {
                if (tmpEngFx.propellants[i].name == MAIN_POWER_NAME)
                {
                    usesCharge = true;
                    ecratio = tmpEngFx.propellants[i].ratio;
                }
                sumRd += tmpEngFx.propellants[i].ratio * PartResourceLibrary.Instance.GetDefinition(tmpEngFx.propellants[i].id).density;
            }
            if (usesCharge)
            {
                float massFlowRate = 0;

                if ((Utilities.GameModeisFlight && tmpEngFx.isOperational && tmpEngFx.currentThrottle > 0) ||
                    Utilities.GameModeisEditor)
                {
                    if (Utilities.GameModeisEditor)
                        massFlowRate = tmpEngFx.maxThrust/(tmpEngFx.atmosphereCurve.Evaluate(0)*grav);
                    else
                        massFlowRate = tmpEngFx.currentThrottle*tmpEngFx.maxThrust/
                                       (tmpEngFx.atmosphereCurve.Evaluate(0)*grav);

                    if (Utilities.GameModeisEditor || Utilities.GameModeisFlight)
                    {
                        tmpPower = ecratio * massFlowRate / sumRd;
                        prtActive = true;
                    }
                    else
                    {
                        tmpPower = 0;
                        prtActive = false;
                    }
                }
                else
                {
                    tmpPower = 0;
                    prtActive = false;
                }
                AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, module.moduleName, false, prtActive, tmpPower, false, false);
            }
            tmpcurrentEngActive = tmpEngFx.isOperational && (tmpEngFx.currentThrottle > 0);
            if (altRate > 0 && tmpcurrentEngActive)
            {
                //Utilities.Log_Debug("totalPowerProduced ModEngine Active Power = " + altRate + " Part = " + currentPart.name);

                prtActive = true;
                AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, module.moduleName, false, prtActive, (float)altRate, true, false);
            }
            return tmpcurrentEngActive;
        }
        
        private double ProcessModuleAlternator(string prtName, Part currentPart, PartModule module, bool currentEngActive)
        {
            prtActive = false;
            prtPower = "";
            tmpPower = 0f;
            tmpAlt = (ModuleAlternator)module;
            double ECConsumed = 0f;
            double ECProduced = 0f;
            ProcessResHandler("Output", module, out ECConsumed, out ECProduced);
            if (Utilities.GameModeisEditor || (Utilities.GameModeisFlight && currentEngActive))
            {
                //Utilities.Log_Debug("totalPowerProduced ModAlt Active Power = " + r.rate + " Part = " + currentPart.name);
                prtActive = true;
            }
            else
                tmpaltRate = ECProduced;
                    
            AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, module.moduleName, false, prtActive, (float)ECProduced, ECProduced >= 0, false);
                
            return tmpaltRate;
        }

        private void ProcessModuleScienceLab(string prtName, Part currentPart, PartModule module)
        {
            prtActive = false;
            prtPower = "";
            tmpPower = 0f;
            prtActive = false;
            ModuleScienceLab tmpLab = (ModuleScienceLab)module;
            if (Utilities.GameModeisFlight)
            {
                for (int i = tmpLab.processResources.Count - 1; i >= 0; --i)
                {
                    if (tmpLab.processResources[i].name == MAIN_POWER_NAME && tmpLab.IsOperational() && tmpLab.processResources[i].currentAmount > 0.0)
                    {
                        prtActive = true;
                        tmpPower += tmpLab.processResources[i].currentAmount;
                    }
                }
            }
            if (Utilities.GameModeisEditor)
            {
                prtActive = true;
                tmpPower += tmpLab.processResources.Where(r => r.name == MAIN_POWER_NAME).Sum(r => r.amount);
            }
            AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, module.moduleName, false, prtActive, tmpPower, false, false);
        }

        private void ProcessModuleScienceConverter(string prtName, Part currentPart, PartModule module)
        {
            prtActive = false;
            prtPower = "";
            tmpPower = 0f;
            prtActive = false;
            if (Utilities.GameModeisFlight)
            {
                ModuleScienceConverter tmpcnv = (ModuleScienceConverter)module;
                BaseConverter tmpbsecnv = (BaseConverter)module;
                if (tmpbsecnv.IsActivated)
                {
                    prtActive = true;
                    tmpPower = tmpcnv.powerRequirement;
                }
            }
            if (Utilities.GameModeisEditor)
            {
                ModuleScienceConverter tmpcnv = (ModuleScienceConverter)module;
                prtActive = true;
                tmpPower = tmpcnv.powerRequirement;
            }
            AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, module.moduleName, false, prtActive, tmpPower, false, false);
        }

        private void ProcessModuleResourceharvester(string prtName, Part currentPart, PartModule module)
        {
            prtActive = false;
            prtPower = "";
            tmpPower = 0f;
            //double tmpPwr = 0;
            //Utilities.Log_Debug("Resource Harvester " + currentPart.name);
            ModuleResourceHarvester tmpHvstr = (ModuleResourceHarvester)module;
            List<PartResourceDefinition> rscse = tmpHvstr.GetConsumedResources();
            double ratio = tmpHvstr.eInput.Ratio;
            double efficiency = 1;

            if (Utilities.GameModeisFlight)
            {
                prtActive = tmpHvstr.ModuleIsActive();
                efficiency = tmpHvstr.GetEfficiencyMultiplier();
                //Utilities.Log_Debug("Inflight andIsactive = " + prtActive);
            }
            if (Utilities.GameModeisEditor)
            {
                prtActive = true;
                //Utilities.Log_Debug("In VAB and editorMaxECusage is on so part active");
            }

            if (tmpHvstr.eInput.ResourceName == MAIN_POWER_NAME && prtActive)
            {
                tmpPower = ratio * efficiency;
            }
            AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, module.moduleName, false, prtActive, tmpPower, false, false);

            ProcessPartEmergencyShutdownProcedures(currentPart, module, prtActive);
        }
        
        private void ProcessModuleResourceConverter(string prtName, Part currentPart, PartModule module, float KPBS, string mod_num = null)
        {
            prtActive = false;
            prtPower = "";
            tmpPower = 0f;
            tmpRegRc = (ModuleResourceConverter)module;

            string longModuleName = tmpRegRc.moduleName + " " + tmpRegRc.ConverterName + " " + mod_num;

            recipe = tmpRegRc.Recipe;
            //Utilities.Log_Debug("Resource Converter " + prtName);
            double efficiency = 1;
            if (Utilities.GameModeisFlight)
            {
                efficiency = tmpRegRc.GetEfficiencyMultiplier();
                Utilities.ISRUStatus status = Utilities.GetModResConverterStatus(tmpRegRc);
                if (status == Utilities.ISRUStatus.Active) prtActive = true;
                //Utilities.Log_Debug("Inflight andIsactive = {0}, status - {1}" , prtActive , status);
                //Utilities.Log_Debug("Status : {0}, Determined Status : {1}", tmpRegRc.status, status.ToString());
            }
            if (Utilities.GameModeisEditor)
            {
                prtActive = true;
            }

            prtPower = "";
            tmpPower = 0f;
            FillAmount = recipe.FillAmount;
            if (KPBS > 0) FillAmount = KPBS;


            recInputs = tmpRegRc.Recipe.Inputs;
            for (int i = recInputs.Count - 1; i >= 0; --i)
            {
                if (recInputs[i].ResourceName == MAIN_POWER_NAME)
                {
                    //Utilities.Log_Debug("Converter Input resource = " + r.ResourceName + " ratio = " + r.Ratio);
                    if (prtActive)
                        tmpPower = recInputs[i].Ratio * FillAmount * efficiency;
                    AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, longModuleName, false, prtActive, tmpPower, false, false);
                }
            }

            prtPower = "";
            tmpPower = 0f;
            recOutputs = tmpRegRc.Recipe.Outputs;
            for (int i = recOutputs.Count - 1; i >= 0; --i)
            {
                if (recOutputs[i].ResourceName == MAIN_POWER_NAME)
                {
                    //Utilities.Log_Debug("Converter Output resource = " + r.ResourceName + " ratio = " + r.Ratio);
                    if (prtActive)
                        tmpPower = recOutputs[i].Ratio * recipe.TakeAmount * efficiency;
                    AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, longModuleName, false, prtActive, tmpPower, true, false);
                }
            }

            ProcessPartEmergencyShutdownProcedures(currentPart, module, prtActive);
        }
        
        private void ProcessModuleEnviroSensor(string prtName, Part currentPart, PartModule module)
        {
            prtActive = false;
            prtPower = "";
            tmpPower = 0f;
            tmpEnvS = (ModuleEnviroSensor) module;
            if (Utilities.GameModeisFlight && tmpEnvS.sensorActive || Utilities.GameModeisEditor)
            {
                prtActive = true;
                double ECConsumed = 0f;
                double ECProduced = 0f;
                ProcessResHandler("Input", module, out ECConsumed, out ECProduced);
                tmpPower = ECConsumed;
                
                //tmpPower = tmpEnvS.powerConsumption;
            }
            AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, module.moduleName, false, prtActive, tmpPower, false, false);

            ProcessPartEmergencyShutdownProcedures(currentPart, module, prtActive);
        }

        private void ProcessResHandler(String type, PartModule module, out double ECConsumed, out double ECProduced)
        {
            ECConsumed = 0f;
            ECProduced = 0f;
            if (type == "Input" || type == "Both")
            {
                int count = module.resHandler.inputResources.Count;
                for (int i = 0; i < count; i++)
                {
                    if (module.resHandler.inputResources[i].name == MAIN_POWER_NAME)
                    {
                        if (Utilities.GameModeisEditor)
                        {
                            ECConsumed += module.resHandler.inputResources[i].rate;
                        }
                        else
                        {
                            ECConsumed += module.resHandler.inputResources[i].currentAmount;
                        }
                    }
                }
            }

            if (type == "Output" || type == "Both")
            {
                int count2 = module.resHandler.outputResources.Count;
                for (int i = 0; i < count2; i++)
                {
                    if (module.resHandler.outputResources[i].name == MAIN_POWER_NAME)
                    {
                        if (Utilities.GameModeisEditor)
                        {
                            ECProduced += module.resHandler.outputResources[i].rate;
                        }
                        else
                        {
                            ECProduced += module.resHandler.outputResources[i].currentAmount;
                        }
                    }
                }
            }
        }

        #endregion StockPartModules

        #region OtherMods
        private bool KKPresent = false;
        private bool ALPresent = false;
        private bool NFEPresent = false;
        private bool NFSPresent = false;
        private bool NFPPresent = false;
        private bool KASPresent = false;
        private bool _rt2Present = false;
        private bool ScSPresent = false;
        private bool TelPresent = false;
        private bool TACLPresent = false;
        private bool KISEPresent = false;
        private bool AntRPresent = false;
        private bool TFCPresent = false;
        private bool DFPresent = false;
        private bool KPBSPresent = false;
        private bool USILSPresent = false;
        private bool IONRCSPresent = false;
        private bool KERBALISMPresent = false;

        internal void ProcessModPartModule(Part currentPart, PartModule module)
        {
            if (KASPresent)
                try
                {
                    checkKAS(module, currentPart);
                }
                catch
                {
                    Utilities.Log("Wrong KAS library version - disabled.");
                    //KASPresent = false;
                }

            if (_rt2Present)
                try
                {
                    checkRT2(module, currentPart);
                }
                catch
                {
                    Utilities.Log("Wrong Remote Tech 2 library version - disabled.");
                    //RT2Present = false;
                }

            if (ALPresent)
                try
                {
                    checkAv(module, currentPart);
                }
                catch
                {
                    Utilities.Log_Debug("Wrong Aviation Lights library version - disabled.");
                    //ALPresent = false;
                }

            if (NFEPresent)
                try
                {
                    checkNFE(module, currentPart);
                }
                catch
                {
                    Utilities.Log("Wrong Near Future library version - disabled.");
                    //NFEPresent = false;
                }
            if (NFSPresent)
                try
                {
                    checkNFS(module, currentPart);
                }
                catch
                {
                    Utilities.Log_Debug("Wrong Near Future solar library version - disabled.");
                    //NFSPresent = false;
                }
            if (NFPPresent)
                try
                {
                    checkNFP(module, currentPart);
                }
                catch
                {
                    Utilities.Log_Debug("Wrong Near Future Propulsion library version - disabled.");
                    //NFSPresent = false;
                }

            if (ScSPresent)
                try
                {
                    checkSCANsat(module, currentPart);
                }
                catch
                {
                    Utilities.Log("Wrong SCANsat library version - disabled.");
                    ScSPresent = false;
                }

            if (TelPresent)
                try
                {
                    checkTel(module, currentPart);
                }
                catch
                {
                    Utilities.Log("Wrong Telemachus library version - disabled.");
                    TelPresent = false;
                }

            if (AntRPresent)
                try
                {
                    checkAntR(module, currentPart);
                }
                catch
                {
                    Utilities.Log("Wrong AntennaRange library version - disabled.");
                    //AntRPresent = false;
                }

            if (TACLPresent)
                try
                {
                    checkTACL(module, currentPart, false);
                }
                catch
                {
                    Utilities.Log("Wrong TACLS library version - disabled.");
                    //TACLPresent = false;
                }

            if (TFCPresent)
                try
                {
                    checkTFC(module, currentPart);
                }
                catch
                {
                    Utilities.Log("Wrong ToggleFuelCell version - disabled.");
                    TFCPresent = false;
                }

            if (KISEPresent)
                try
                {
                    checkKSPIE(module, currentPart);
                }
                catch
                {
                    //RSTUtils.Utilities.Log("Wrong Interstellar version - disabled.");
                    //KISEPresent = false;
                }
            if (DFPresent)
                try
                {
                    checkDF(module, currentPart);
                }
                catch
                {
                    Utilities.Log("Wrong DeepFreeze version - disabled.");
                    DFPresent = false;
                }
            if (KPBSPresent)
                try
                {
                    checkKPBS(module, currentPart);
                }
                catch
                {
                    Utilities.Log("Wrong KPBS version - disabled.");
                    KPBSPresent = false;
                }
            if (USILSPresent)
                try
                {
                    checkUSILS(module, currentPart);
                }
                catch
                {
                    Utilities.Log("Wrong USI LS version - disabled.");
                    //KPBSPresent = false;
                }
            if (KERBALISMPresent)
                try
                {
                    checkKerbalism(module, currentPart);
                }
                catch (Exception)
                {
                    Utilities.Log("Wrong kerbalism version - disabled.");
                    KERBALISMPresent = false;
                }
        
            /*if (KKPresent)
            {
                checkKK(module, current_part);
            }*/
        }
        
        private void checkAv(PartModule psdpart, Part currentPart)
        {
            prtName = currentPart.name;
            prtPower = "";
            prtActive = false;
            tmpPower = 0;
            switch (psdpart.moduleName)
            {
                case "ModuleNavLight":
                    ALtmpLight = new ALWrapper.ALNavLight(psdpart);
                    if ((Utilities.GameModeisFlight && ALtmpLight.navLightSwitch != 0) || Utilities.GameModeisEditor)
                    {
                        prtActive = true;
                        tmpPower = ALtmpLight.EnergyReq;
                        AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, false, false);
                    }

                    ProcessPartEmergencyShutdownProcedures(currentPart, psdpart, prtActive);
                    break;
            }
        }
        
        private void checkNFE(PartModule psdpart, Part currentPart)
        {
            prtName = currentPart.name;
            prtPower = "";
            prtActive = false;
            tmpPower = 0;
            switch (psdpart.moduleName)
            {
                case "FissionReactor":
                    tmpRegRc = (ModuleResourceConverter)psdpart;
                    prtName = currentPart.name;
                    if (Utilities.GameModeisFlight)
                    {
                        prtActive = tmpRegRc.ModuleIsActive();
                    }
                    if (Utilities.GameModeisEditor)
                    {
                        prtActive = true;
                        //Utilities.Log_Debug("In VAB and editorMaxECusage is on so part active");
                    }
                    
                    prtPower = "";
                    tmpPower = 0f;
                    recInputs = tmpRegRc.Recipe.Inputs;
                    for (int i = recInputs.Count - 1; i >= 0; --i)
                    {
                        
                        if (recInputs[i].ResourceName == MAIN_POWER_NAME && prtActive)
                        {
                            //Utilities.Log_Debug("Converter Input resource = " + r.ResourceName + " ratio = " + r.Ratio);
                            tmpPower = recInputs[i].Ratio;
                            AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, false, false);
                        }
                    }

                    prtPower = "";
                    tmpPower = 0f;
                    recOutputs = tmpRegRc.Recipe.Outputs;
                    for (int i = recOutputs.Count - 1; i >= 0; --i)
                    {
                        if (recOutputs[i].ResourceName == MAIN_POWER_NAME && prtActive)
                        {
                            //Utilities.Log_Debug("Converter Output resource = " + r.ResourceName + " ratio = " + r.Ratio);
                            tmpPower = recOutputs[i].Ratio;
                            AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, true, false);
                        }
                    }
                    break;

                case "FissionGenerator":
                    try
                    {
                        fieldlist = psdpart.Fields;
                        state = fieldlist.GetValue("GeneratorStatus");
                        generatorState = (string)state;
                        if (generatorState != "Offline")
                        {
                            prtActive = true;
                            object objPower = null;
                            if (Utilities.GameModeisFlight)
                            {
                                objPower = fieldlist.GetValue("CurrentGeneration");
                            }
                            if (Utilities.GameModeisEditor)
                            {
                                objPower = fieldlist.GetValue("PowerGeneration");
                            }
                            if (objPower != null)
                                tmpPower = (float)objPower;
                        }
                        AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, true, false);
                    }
                    catch (Exception)
                    {
                        //throw;
                    }
                    break;

                case "FissionFlowRadiator":
                    ProcessModuleActiveRadiator(prtName, currentPart, psdpart);
                    break;

                case "ModuleRadioisotopeGenerator":
                    try
                    {
                        fieldlist = psdpart.Fields;
                        ec_rate = fieldlist.GetValue("ActualPower");
                        tmpPower = (float)ec_rate;
                        if (tmpPower > 0)
                        {
                            prtActive = true;
                        }
                        AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, true, false);
                    }
                    catch (Exception)
                    {
                        //throw;
                    }
                    
                    break;

                case "DischargeCapacitor":
                    try
                    {
                        fieldlist = psdpart.Fields;
                        state = fieldlist.GetValue("CapacitorStatus");
                        generatorState = (string)state;
                        if (generatorState.Contains("Discharging:"))
                        {
                            prtActive = true;
                            stringlength = generatorState.Length - 15;
                            generatorState = generatorState.Substring(13, stringlength);
                            if (!double.TryParse(generatorState, out tmpPower))
                                tmpPower = 0.0f;
                            AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, true, false);
                        }
                        if (generatorState.Contains("Recharging:"))
                        {
                            prtActive = true;
                            stringlength = generatorState.Length - 14;
                            generatorState = generatorState.Substring(12, stringlength);
                            if (!double.TryParse(generatorState, out tmpPower))
                                tmpPower = 0.0f;
                            AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, false, false);
                        }
                    }
                    catch (Exception)
                    {
                        //throw;
                    }
                    
                    break;
            }
        }
        
        private void checkNFS(PartModule psdpart, Part currentPart)
        {
            prtName = currentPart.name;
            prtPower = "";
            prtActive = false;
            tmpPower = 0;
            switch (psdpart.moduleName)
            {
                case "Curved Solar Panel":
                    NFSCPtmpGen = new NFSWrapper.NFSCurvedPanel(psdpart);
                    if (Utilities.GameModeisFlight && NFSCPtmpGen.State == ModuleDeployablePart.DeployState.EXTENDED)
                    {
                        prtActive = true;
                        tmpPower = NFSCPtmpGen.EnergyFlow;
                    }
                    else if (Utilities.GameModeisEditor)
                    {
                        prtActive = true;
                        tmpPower = NFSCPtmpGen.TotalEnergyRate;
                        //Utilities.Log_Debug("In VAB and editorMaxECusage is on so part active");
                    }
                    AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, true, true);
                    ProcessPartEmergencyShutdownProcedures(currentPart, psdpart, prtActive);
                    break;
            }
        }
        
        private void checkKAS(PartModule psdpart, Part currentPart)
        {
            prtName = currentPart.name;
            prtPower = "";
            prtActive = false;
            tmpPower = 0;
            switch (psdpart.moduleName)
            {
                case "KASModuleWinch":
                    tmpKw = new KASWrapper.KASModuleWinch(psdpart);
                    if (Utilities.GameModeisFlight && tmpKw.isActive && tmpKw.motorSpeed > 0f)
                    {
                        tmpPower = tmpKw.powerDrain * tmpKw.motorSpeed;
                        prtActive = true;
                    }
                    if (Utilities.GameModeisEditor)
                    {
                        prtActive = true;
                        tmpPower = tmpKw.powerDrain;

                        //Utilities.Log_Debug("In VAB and editorMaxECusage is on so part active");
                    }
                    AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, false, false);

                    break;

                case "KASModuleMagnet":
                    tmpKm = new KASWrapper.KASModuleMagnet(psdpart);
                    prtActive = false;
                    tmpPower = 0;

                    if (Utilities.GameModeisEditor || (Utilities.GameModeisFlight && tmpKm.MagnetActive))
                    {
                        prtActive = true;
                        tmpPower = tmpKm.powerDrain;
                    }
                    AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, false, false);

                    break;
            }
        }
        
        private void checkRT2(PartModule psdpart, Part currentPart)
        {
            switch (psdpart.moduleName)
            {
                case "ModuleRTAntenna":
                    prtName = currentPart.name;
                    prtPower = "";
                    prtActive = false;
                    tmpPower = 0;
                    RTtmpAnt = new RTWrapper.RTAntenna(psdpart);
                    if (Utilities.GameModeisFlight && RTtmpAnt.Activated)
                    {
                        //Utilities.Log_Debug("RTAntenna consumption " + RTtmpAnt.Consumption);

                        prtActive = true;
                        tmpPower = RTtmpAnt.Consumption;
                    }
                    if (Utilities.GameModeisEditor)
                    {
                        prtActive = true;
                        tmpPower = RTtmpAnt.EnergyCost;

                        //Utilities.Log_Debug("In VAB and editorMaxECusage is on so part active");
                        //Utilities.Log_Debug("tmpant2 energycost " + RTtmpAnt.EnergyCost);
                    }
                    AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, false, false);

                    ProcessPartEmergencyShutdownProcedures(currentPart, psdpart, prtActive);

                    break;
            }
        }
        
        private void checkSCANsat(PartModule psdpart, Part currentPart)
        {
            switch (psdpart.moduleName)
            {
                case "ModuleSCANresourceScanner":
                case "SCANsat":
                    prtName = currentPart.name;
                    prtPower = "";
                    prtActive = false;
                    tmpPower = 0;
                    tmpSs = new ScanSatWrapper.SCANsat(psdpart);

                    double ECConsumed = 0f;
                    double ECProduced = 0f;
                    ProcessResHandler("Input", psdpart, out ECConsumed, out ECProduced);

                    if (Utilities.GameModeisEditor || (Utilities.GameModeisFlight && tmpSs.scanning))
                    {
                        prtActive = true;
                        tmpPower = ECConsumed;
                    }
                    AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, false, false);

                    ProcessPartEmergencyShutdownProcedures(currentPart, psdpart, prtActive);

                    break;
            }
        }
        
        private void checkTel(PartModule psdpart, Part currentPart)
        {
            switch (psdpart.moduleName)
            {
                case "TelemachusPowerDrain":
                    prtName = "Telemachus";
                    prtPower = "";
                    prtActive = false;
                    tmpPower = 0;
                    tmpTm = new TeleWrapper.TMPowerDrain(psdpart);
                    if (Utilities.GameModeisFlight && tmpTm.isActive)
                    {
                        prtPower = tmpTm.powerConsumption.ToString("000.00000");
                        prtActive = true;
                        tmpPower = tmpTm.powerConsumption;
                    }
                    if (Utilities.GameModeisEditor)
                    {
                        prtActive = true;
                        tmpPower = 0.01f;

                        prtPower = "0.010";
                        //Utilities.Log_Debug("Telemachus In VAB and editorMaxECusage is on so part active");
                    }
                    AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, false, false);


                    break;
            }
        }
        
        private void checkTACL(PartModule psdpart, Part currentPart, bool cmdPod)
        {
            if (TACLSWrapper.TACactualAPI.getEnabled())
            {
                if (cmdPod)
                {
                    prtName = "TAC Life Support";
                    prtPower = "";
                    prtActive = false;
                    if (Utilities.GameModeisFlight) //if in flight set maxCrew to actual crew on board. Set earlier based on maximum crew capacity of each part
                    {
                        MaxCrew = FlightGlobals.ActiveVessel.GetCrewCount();
                    }

                    double calcDrain = 0;
                    calcDrain = TACLSWrapper.TACactualAPI.BaseElectricityConsumptionRate * crewablePartList.Count;
                    calcDrain += TACLSWrapper.TACactualAPI.ElectricityConsumptionRate * MaxCrew;

                    prtActive = MaxCrew > 0;

                    AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, "LifeSupport", false, prtActive, (float)calcDrain, false, false);
                }
                else
                {
                    prtName = currentPart.name;
                    prtPower = "";
                    prtActive = false;
                    tmpPower = 0;
                    switch (psdpart.moduleName)
                    {
                        case "TacGenericConverter":
                            ProcessModuleResourceConverter(currentPart.name, currentPart, psdpart, 0);
                            break;
                    }
                }
            }
        }

        private void checkAntR(PartModule psdpart, Part currentPart)
        {
            switch (psdpart.moduleName)
            {
                case "ModuleLimitedDataTransmitter":
                    prtName = currentPart.name;
                    prtPower = "";
                    prtActive = false;
                    tmpPower = 0;
                    tmpAnt = (ModuleDataTransmitter)psdpart;

                    if (Utilities.GameModeisEditor || (Utilities.GameModeisFlight && tmpAnt.IsBusy()))
                    {
                        prtActive = true;
                        tmpPower = tmpAnt.DataResourceCost * (1 / tmpAnt.packetInterval);
                    }
                    AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, false, false); ;

                    ProcessPartEmergencyShutdownProcedures(currentPart, psdpart, prtActive);

                    break;
            }
        }

        private void checkTFC(PartModule psdpart, Part currentPart)
        {
            switch (psdpart.moduleName)
            {
                case "ModuleFuelCell":
                    prtName = currentPart.name;
                    prtPower = "";
                    prtActive = false;
                    tmpPower = 0;
                    tmpRegRc = (ModuleResourceConverter)psdpart;

                    //Utilities.Log_Debug("Resource Converter " + prtName);

                    if (Utilities.GameModeisFlight)
                    {
                        prtActive = tmpRegRc.ModuleIsActive();
                    }
                    if (Utilities.GameModeisEditor)
                    {
                        prtActive = true;
                    }

                    prtPower = "";
                    tmpPower = 0f;
                    recInputs = tmpRegRc.Recipe.Inputs;
                    for (int i = recInputs.Count - 1; i >= 0; --i)
                    {
                        if (recInputs[i].ResourceName == MAIN_POWER_NAME && prtActive)
                        {
                            tmpPower = recInputs[i].Ratio;
                            //Utilities.Log_Debug("Converter Input resource = " + r.ResourceName + " ratio = " + r.Ratio);
                            AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, false, false);
                        }
                    }

                    prtPower = "";
                    tmpPower = 0f;
                    recOutputs = tmpRegRc.Recipe.Outputs;
                    for (int i = recOutputs.Count - 1; i >= 0; --i)
                    {
                        if (recOutputs[i].ResourceName == MAIN_POWER_NAME && prtActive)
                        {
                            tmpPower = recOutputs[i].Ratio;
                            //Utilities.Log_Debug("Converter Output resource = " + r.ResourceName + " ratio = " + r.Ratio);
                            AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, true, false);
                        }
                    }
                    break;
            }
        }
        
        private void checkKSPIE(PartModule psdpart, Part currentPart)
        {
            prtName = currentPart.name;
            prtPower = "";
            prtActive = false;
            tmpPower = 0;
            switch (psdpart.moduleName)
            {
                case "FNModuleCryostat":
                    tmpCryo = new KSPIEWrapper.FNModuleCryostat(psdpart);

                    if (Utilities.GameModeisFlight && !tmpCryo.isDisabled)
                    {
                        prtPower = tmpCryo.recievedPowerKW.ToString("####000.00");
                        prtActive = true;
                        tmpPower = tmpCryo.recievedPowerKW;
                    }
                    if (Utilities.GameModeisEditor)
                    {
                        prtActive = true;
                        tmpPower = tmpCryo.powerReqKW;
                        prtPower = tmpCryo.powerReqKW.ToString("####000.00");
                        //Utilities.Log_Debug("KSPIE Cryostat In VAB and editorMaxECusage is on so part active");
                    }
                    if (prtActive)
                    {
                        AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, false, false);
                    }
                    break;

                case "FNGenerator":
                    FNtmpGen = new KSPIEWrapper.FNGenerator(psdpart);

                    if (Utilities.GameModeisFlight && FNtmpGen.IsEnabled)
                    {
                        prtPower = FNtmpGen.outputPower.ToString("####000.00");
                        prtActive = true;
                        tmpPower = FNtmpGen.outputPower;
                    }
                    if (Utilities.GameModeisEditor)
                    {
                        prtActive = true;
                        tmpPower = FNtmpGen.maxThermalPower;
                        prtPower = FNtmpGen.maxThermalPower.ToString("####000.00");
                        //Utilities.Log_Debug("KSPIE Cryostat In VAB and editorMaxECusage is on so part active");
                    }
                    if (prtActive)
                    {
                        AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, true, false);
                    }
                    break;
            }
        }
        
        private void checkDF(PartModule psdpart, Part currentPart)
        {
            switch (psdpart.moduleName)
            {
                case "DeepFreezer":
                    prtName = currentPart.name;
                    prtPower = "";
                    prtActive = false;
                    tmpPower = 0;
                    tmpDeepFreezer = new DFWrapper.DeepFreezer(psdpart);

                    //Utilities.Log_Debug("DeepFreezer " + prtName);
                    prtPower = "";
                    tmpPower = 0f;
                    if (Utilities.GameModeisFlight)
                    {
                        if (tmpDeepFreezer.ECReqd && tmpDeepFreezer.TotalFrozen > 0)
                        {
                            prtActive = true;
                            tmpPower = tmpDeepFreezer.FrznChargeUsage;
                        }
                        else
                        {
                            prtActive = false;
                        }
                    }
                    if (Utilities.GameModeisEditor)
                    {
                        if (tmpDeepFreezer.ECReqd)
                        {
                            prtActive = true;
                            tmpPower = tmpDeepFreezer.FreezerSize * tmpDeepFreezer.FrznChargeRequired / 60 * Time.fixedDeltaTime;
                        }
                        else
                        {
                            prtActive = false;
                        }
                        
                    }
                    AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, false, false);
                    break;
            }
        }
        
        private void checkKPBS(PartModule psdpart, Part currentPart)
        {
            switch (psdpart.moduleName)
            {
                case "ModuleKPBSConverter":
                    KPBSconv = new KPBSWrapper.ModuleKPBSConverter(psdpart);
                    currentRateConverter = KPBSconv.currentRate;
                    ProcessModuleResourceConverter(currentPart.name, currentPart, psdpart, currentRateConverter);
                    
                    break;

                case "PlanetaryGreenhouse":
                    KPBSgh = new KPBSWrapper.PlanetaryGreenhouse(psdpart);
                    currentRateGreenhouse = KPBSgh.currentRate;
                    ProcessModuleResourceConverter(currentPart.name, currentPart, psdpart, currentRateGreenhouse);

                    break;
            }
        }

        private void checkUSILS(PartModule psdpart, Part currentPart)
        {
            //Only parse once if ModuleSwappableConverter is present
            if (psdpart.moduleName != "ModuleSwappableConverter" && currentPart.Modules.Contains("ModuleSwappableConverter"))
            {
                return;
            }

            switch (psdpart.moduleName)
            {
                case "ModuleSwappableConverter":

                    var converterList = currentPart.Modules.GetModules<BaseConverter>();
                    var bayList = currentPart.Modules.GetModules<PartModule>();

                    foreach (var bay in bayList)
                    {
                        if (bay.moduleName == "ModuleSwappableConverter")
                        {
                            usiMSC = new USILSWrapper.ModuleSwappableConverter(bay);
                            var converter = converterList[usiMSC.currentLoadout];
                            string bayname = usiMSC.bayName;
                            if (converter != null)
                            {
                                switch (converter.moduleName)
                                {
                                    case "ModuleLifeSupportRecycler":
                                    case "ModuleHabitation":
                                    case "ModuleLifeSupportExtender":
                                    case "ModuleResourceConverter_USI":
                                    case "ModuleEfficiencyPart":
                                    case "ModuleBulkConverter":
                                        ProcessModuleResourceConverter(currentPart.name, currentPart, converter, 0, bayname);
                                        break;
                                    case "ModuleRessourceHarvester_USI":
                                        ProcessModuleResourceharvester(currentPart.name, currentPart, converter);
                                        break;
                                    case "ModuleHeatPump":
                                        ProcessModuleActiveRadiator(currentPart.name, currentPart, psdpart);
                                        break;
                                    default:
                                        Utilities.Log("MSC: " + converter.moduleName + " (" + currentPart.name + ")");
                                        break;
                                }
                            }
                        }
                    }
                    break;
                case "ModuleLifeSupportRecycler":
                case "ModuleHabitation":
                case "ModuleLifeSupportExtender":
                case "ModuleResourceConverter_USI":
                case "ModuleEfficiencyPart":
                case "ModuleBulkConverter":
                    ProcessModuleResourceConverter(currentPart.name, currentPart, psdpart, 0);
                    break;
                case "ModuleRessourceHarvester_USI":
                    ProcessModuleResourceharvester(currentPart.name, currentPart, psdpart);
                    break;
                case "ModuleHeatPump":
                    ProcessModuleActiveRadiator(currentPart.name, currentPart, psdpart);
                    break;
                case "ModuleLifeSupport":
                    prtName = currentPart.name;
                    prtPower = "";
                    prtActive = false;
                    tmpPower = 0;

                    if (Utilities.GameModeisFlight && currentPart.protoModuleCrew.Count > 0)
                    {
                        if (currentPart.vessel != null)
                            usiMLS = new USILSWrapper.ModuleLifeSupportSystem(currentPart.vessel.vesselModules.Find(x => x.GetType() == USILSWrapper.USIMLSSType));
                        if (usiMLS != null)
                        {
                            prtActive = true;
                            recInputs = usiMLS.LifeSupportRecipe.Inputs;
                            for (int i = recInputs.Count - 1; i >= 0; --i)
                            {
                                if (recInputs[i].ResourceName == MAIN_POWER_NAME)
                                {
                                    tmpPower = (recInputs[i].Ratio / currentPart.vessel.GetCrewCount()) * currentPart.protoModuleCrew.Count;
                                    AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, false, false);
                                }
                            }
                        }
                    }
                    if (Utilities.GameModeisEditor)
                    {
                        prtActive = true;
                        tmpPower = 0.01f;//default, how to get ECAmount in editor ?
                        int maxCrew = currentPart.CrewCapacity;
                        AYVesselPartLists.AddPart(10101010, prtName, "Kerbals", "Max", false, prtActive, tmpPower * maxCrew, false, false);
                        //get current crew assigned to craft
                        //AYVesselPartLists.AddPart(10101010, prtName, "Kerbals", "Current", false, prtActive, tmpPower * curCrew, false, false);
                    }
                    break;
            }
        }
        
        private void checkIONRCS(PartModule psdpart, Part currentPart)
        {
            prtName = currentPart.name;
            prtPower = "";
            prtActive = false;
            tmpPower = 0;

            switch (psdpart.moduleName)
            {
                case "ModuleIONPoweredRCS":
                    tmpIonPoweredRcs = new IONRCSWrapper.ModuleIONPoweredRCS(psdpart);
                    IONRCSelecUse = tmpIonPoweredRcs.ElecUsed;

                    prtActive = IONRCSelecUse > 0;
                    if (Utilities.GameModeisEditor)
                    {
                        prtActive = true;
                        IONRCSelecUse = tmpIonPoweredRcs.powerRatio;
                    }
                    AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, IONRCSelecUse, false, false);

                    currentPoweredRCSDrain += IONRCSelecUse;
                    //Utilities.Log_Debug("AYIONRCS ElecUsage = " + IONRCSelecUse.ToString("0.00000000"));

                    ProcessPartEmergencyShutdownProcedures(currentPart, psdpart, prtActive);

                    break;

                case "ModulePPTPoweredRCS":
                    tmpPPTPoweredRcs = new IONRCSWrapper.ModulePPTPoweredRCS(psdpart);
                    IONRCSelecUse = tmpPPTPoweredRcs.ElecUsed;

                    prtActive = IONRCSelecUse > 0;
                    if (Utilities.GameModeisEditor)
                    {
                        prtActive = true;
                        IONRCSelecUse = tmpPPTPoweredRcs.powerRatio;
                    }
                    AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, IONRCSelecUse, false, false);

                    currentPoweredRCSDrain += IONRCSelecUse;
                    //Utilities.Log_Debug("AYPPTRCS ElecUsage = " + IONRCSelecUse.ToString("0.00000000"));

                    ProcessPartEmergencyShutdownProcedures(currentPart, psdpart, prtActive);

                    break;
            }
        }
        
        private void checkKerbalism(PartModule psdpart, Part currentPart)
        {
            
            if (psdpart.moduleName == "GravityRing")
            {
                prtName = currentPart.name;
                prtPower = "";
                prtActive = false;
                tmpPower = 0;
                fieldlist = psdpart.Fields;
                state = fieldlist.GetValue("opened");
                ec_rate = fieldlist.GetValue("ec_rate");
                speed = fieldlist.GetValue("speed");
                prtActive = (bool) state;
                if (Utilities.GameModeisEditor)
                {
                    prtActive = true;
                }
                if (prtActive)
                    tmpPower = (double) ec_rate * (double) speed;

                AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, false, false);

                ProcessPartEmergencyShutdownProcedures(currentPart, psdpart, prtActive);
            }
            if (psdpart.moduleName == "Greenhouse")
            {
                prtName = currentPart.name;
                prtPower = "";
                prtActive = false;
                tmpPower = 0;
                fieldlist = psdpart.Fields;
                lamps = fieldlist.GetValue("lamps");
                ec_rate = fieldlist.GetValue("ec_rate");
                prtActive = (float)lamps > 0.0f;
                if (Utilities.GameModeisEditor)
                {
                    prtActive = true;
                }
                if (prtActive)
                    tmpPower = (double)ec_rate * (double)lamps;

                AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, false, false);

                ProcessPartEmergencyShutdownProcedures(currentPart, psdpart, prtActive);
            }
            if (psdpart.moduleName == "Scrubber")
            {
                prtName = currentPart.name;
                prtPower = "";
                prtActive = false;
                tmpPower = 0;
                fieldlist = psdpart.Fields;
                is_enabled = fieldlist.GetValue("is_enabled");
                ec_rate = fieldlist.GetValue("ec_rate");
                co2_rate = fieldlist.GetValue("co2_rate");

                prtActive = (bool)is_enabled;
                if (Utilities.GameModeisEditor)
                {
                    prtActive = true;
                }
                if (prtActive)
                    tmpPower = (double)ec_rate * (double)co2_rate;

                AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, false, false);

                ProcessPartEmergencyShutdownProcedures(currentPart, psdpart, prtActive);
            }
        }
        
        private void checkNFP(PartModule psdpart, Part currentPart)
        {

            if (psdpart.moduleName == "VariablePowerEngine")
            {
                prtName = currentPart.name;
                prtPower = "";
                prtActive = false;
                tmpPower = 0;
                fieldlist = psdpart.Fields;
                ec_rate = fieldlist.GetValue("curPowerUse");
                prtActive = (float)ec_rate > 0.0f;
                if (Utilities.GameModeisEditor)
                {
                    prtActive = true;
                }
                if (prtActive)
                    tmpPower = (double)ec_rate;

                AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, false, false);
            }
            if (psdpart.moduleName == "VariableISPEngine")
            {
                prtName = currentPart.name;
                prtPower = "";
                prtActive = false;
                tmpPower = 0;
                tmpVariableIspEngine = new NFPWrapper.VariableISPEngine(psdpart);
                tmpPower = tmpVariableIspEngine.ECUsage;
                prtActive = tmpPower > 0.0f;
                if (Utilities.GameModeisEditor)
                {
                    prtActive = true;
                }
                AYVesselPartLists.AddPart(currentPart.craftID, prtName, currentPart.partInfo.title, psdpart.moduleName, false, prtActive, tmpPower, false, false);
            }
        }

        #endregion OtherMods

        #region EmergencyShutdownProcedures

        private double ESPpowerPercent;

        private void ProcessEmergencyShutdownChecking()
        {
            //Check if we are in an Emergency Shutdown CoolDown Period (EmgcyShutOverrideTmeStarted > 0)
            // If we are, continue the clock until the cooldown period is up. During that time no other Emergency Shutdown Processing is done.
            if (EmgcyShutOverrideTmeStarted > 0)
            {
                if (Planetarium.GetUniversalTime() - EmgcyShutOverrideTmeStarted < AYsettings.EmgcyShutOverrideCooldown) //Still in cooldown period
                {
                    return;
                }
                else
                {
                    //Cooldown period is finished.
                    EmgcyShutOverrideTmeStarted = 0;
                }
            }

            //Check for Emergency Shutdown Procedures system
            if (EmgcyShutActive) //Only process if the user has activated the system.
            {
                //Check if a Manual Override is active and process that first.
                if (EmgcyShutOverride)
                {
                    try
                    {
                        Emergencypowerdownreset = false;
                        Emergencypowerdownactivated = true;
                        if (!EmgcyShutOverrideStarted)// This will execute the first loop of a Manual ESP override. Process the Lows first.
                        {
                            _espPriority = ESPPriority.LOW;
                            EmgcyShutOverrideStarted = true;
                            Utilities.Log_Debug("emergency shutdown, _ESPPrioirity={0}", _espPriority);
                            EmergencyShutDownProcedureAmpYearSystems(false, true);
                        }
                        else
                        //This will be true second and third time around after Low priority manual override has executed.
                        {
                            if (_espPriority == ESPPriority.LOW)
                                //When we get here the Lows have had a chance to complete so move on to the mediums.
                            {
                                _espPriority = ESPPriority.MEDIUM;
                                Utilities.Log_Debug("emergency shutdown, _ESPPrioirity={0}", _espPriority);
                                EmergencyShutDownProcedureAmpYearSystems(false, true);
                            }
                            else
                            {
                                if (_espPriority == ESPPriority.MEDIUM)
                                    //When we get here the Lows and Mediums have been completed so move on to the highs.
                                {
                                    _espPriority = ESPPriority.HIGH;
                                    Utilities.Log_Debug("emergency shutdown, _ESPPrioirity={0}", _espPriority);
                                    EmergencyShutDownProcedureAmpYearSystems(false, true);
                                }
                                else
                                // When we get here we have done the Low, Mediums and Highs so ESP Override processing is completed.
                                {
                                    EmgcyShutOverride = false;
                                    EmgcyShutOverrideStarted = false;
                                    Emergencypowerdownactivated = false;
                                    EmgcyShutOverrideTmeStarted = Planetarium.GetUniversalTime();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Utilities.Log("Exception occurred during ESP Override processing");
                        Utilities.Log("Exception: {0}", ex);
                    }
                }
                else //Automatic ESP processing starts here.
                {
                    try
                    {
                        ESPpowerPercent = TotalElectricCharge / TotalElectricChargeCapacity * 100.0;
                        PowerState = TotalPowerProduced >= TotalPowerDrain ? PowerState.Increasing : PowerState.Decreasing;
                        //If power state is decreasing we check if power percentage has gone below the LowThreshold (as it is always the highest percentage)
                        if (PowerState == PowerState.Decreasing)
                        {
                            if (ESPpowerPercent < AYsettings.ESPLowThreshold)  //Are we in an Emergency Power Down State?
                            {
                                //If emergency power down was not previously activated we reset ALL the processing flags at the start of a powerdown.
                                if (!Emergencypowerdownprevactivated)  
                                {
                                    ESPPriorityHighProcessed = ESPPriorityMediumProcessed = ESPPriorityLowProcessed = false;
                                    ESPPriorityHighResetProcessed = ESPPriorityMediumResetProcessed = ESPPriorityLowResetProcessed = false;
                                    Emergencypowerdownreset = Emergencypowerdownresetprevactivated = false;
                                }
                                Emergencypowerdownprevactivated = Emergencypowerdownactivated = true;
                            }
                        }
                        else  // power state is increasing we check power percentage
                        {
                            //First if Power is now increasing we reset the PowerDown flags.
                            Emergencypowerdownprevactivated = Emergencypowerdownactivated = false;
                            ESPPriorityHighProcessed = ESPPriorityMediumProcessed = ESPPriorityLowProcessed = false;

                            //if power percentage has gone below the LowThreshold (as it is always the highest percentage)
                            if (ESPpowerPercent < AYsettings.ESPLowThreshold)   //Are we in an Emergency Power Down Reset state?
                            {
                                Emergencypowerdownreset = true;
                            }
                            else
                            {
                                //If we are here then power is increasing and powerPercent is above the LowThreshold.
                                //So If LowPriority has been processed then reset everything as we are out of danger.
                                if (ESPPriorityLowResetProcessed)
                                {
                                    //Reset ALL flags and Return because we are out of danger.
                                    ESPPriorityHighResetProcessed = ESPPriorityMediumResetProcessed = ESPPriorityLowResetProcessed = false;
                                    Emergencypowerdownreset = Emergencypowerdownresetprevactivated = false;
                                    return;
                                }
                            }
                        }
                        //If we aren't in an Emergency Power Down or Reset state we finish processing here.
                        if (!Emergencypowerdownactivated && !Emergencypowerdownreset)
                        {
                            return;
                        }

                        //Issue a LOG message if somehow both modes are set on.. and cancel both and stop there.
                        if (Emergencypowerdownactivated && Emergencypowerdownreset)
                        {
                            Utilities.Log("Emergency Shutdown Process is in BOTH Shutdown and Restart MODES at the same time, This should NEVER happen");
                            Utilities.Log("Deactivated ESP system");
                            Emergencypowerdownactivated = false;
                            Emergencypowerdownreset = false;
                            EmgcyShutActive = false;
                            ScreenMessages.PostScreenMessage(
                                    "Failure in Emergency Shutdown Process System, ESP System has been deactivated.", 5.0f,
                                    ScreenMessageStyle.UPPER_CENTER);
                            return;
                        }
                        //If we get here we are either dong Emergency Power Down or Emergency Reset
                        //Now set the Priority
                        if (ESPpowerPercent < AYsettings.ESPHighThreshold)  //Check the lowest percentage first, which is the high priority parts.
                        {
                            _espPriority = ESPPriority.HIGH;
                        }
                        else
                        {
                            _espPriority = ESPpowerPercent < AYsettings.ESPMediumThreshold ? ESPPriority.MEDIUM : ESPPriority.LOW;
                        }

                        //Process EmergencyPowerdownactivated for AmpYear subsystems
                        if (Emergencypowerdownactivated)
                        {
                            if ((_espPriority == ESPPriority.HIGH && !ESPPriorityHighProcessed)
                                || (_espPriority == ESPPriority.MEDIUM && !ESPPriorityMediumProcessed)
                                || (_espPriority == ESPPriority.LOW && !ESPPriorityLowProcessed)
                                )
                            {
                                Utilities.Log_Debug("emergency shutdown, _ESPPrioirity={0}", _espPriority);
                                EmergencyShutDownProcedureAmpYearSystems(false, true);
                            }

                        }

                        //Process EmergencyReset for AmpYear subsystems
                        if (Emergencypowerdownreset)
                        {
                            if ((_espPriority == ESPPriority.HIGH && !ESPPriorityHighResetProcessed)
                                || (_espPriority == ESPPriority.MEDIUM && !ESPPriorityMediumResetProcessed)
                                || (_espPriority == ESPPriority.LOW && !ESPPriorityLowResetProcessed)
                                )
                            {
                                Utilities.Log_Debug("emergency restart, _ESPPrioirity={0}", _espPriority);
                                EmergencyShutDownProcedureAmpYearSystems(false, true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Utilities.Log("Exception occurred during ESP processing");
                        Utilities.Log("Exception: {0}", ex);
                    }
                }
            }
            else //System is deactivated
            {
                Emergencypowerdownreset = Emergencypowerdownresetprevactivated = false;
                Emergencypowerdownactivated = Emergencypowerdownprevactivated = false;
                EmgcyShutOverride = false;
                ESPPriorityHighProcessed = ESPPriorityMediumProcessed = ESPPriorityLowProcessed = false;
                ESPPriorityHighResetProcessed = ESPPriorityMediumResetProcessed = ESPPriorityLowResetProcessed = false;
            }
        }

        private void EmergencyShutDownProcedureAmpYearSystems(bool Manager, bool Subsystems)
        {
            string prtName = "AmpYear Subsystems-Max";
            string prtPower = "";
            uint partId = CalcAmpYearMgrPartID();

            if (Manager)
            {
                PwrPartList partFnd;
                string keyValue = AYVesselPartLists.CreatePartKey(partId, "AYSubsystems");
                if (AYVesselPartLists.VesselConsPartsList.TryGetValue(keyValue, out partFnd))
                {
                    if (partFnd.ValidprtEmergShutDn && partFnd.PrtEmergShutDnInclude)
                    {
                        if (Emergencypowerdownactivated) //Emergency ShutDown Procedures
                        {
                            partFnd.PrtPreEmergShutDnStateActive = _managerEnabled;
                            AYVesselPartLists.VesselConsPartsList[keyValue] = partFnd;
                            if (partFnd.PrtActive)
                                _managerEnabled = false;
                        }
                        if (Emergencypowerdownreset) //Emergency Reset Procedures
                        {
                            if (partFnd.PrtPreEmergShutDnStateActive && !partFnd.PrtActive)
                            {
                                _managerEnabled = true;
                            }
                        }
                    }
                }
            }

            if (Subsystems)
            {
                try
                {
                    if (Emergencypowerdownactivated) //Emergency ShutDown Procedures
                    {
                        TimeWarp.SetRate(0, false);
                        ScreenMessages.PostScreenMessage(FlightGlobals.ActiveVessel.vesselName + " - Emergency Shutdown Procedures Activated. Shutdown Subsystems.", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                    }
                    if (Emergencypowerdownreset) //Emergency Reset Procedures
                        ScreenMessages.PostScreenMessage(FlightGlobals.ActiveVessel.vesselName + " - Emergency Shutdown Procedures Activated. Restart Subsystems.", 5.0f, ScreenMessageStyle.UPPER_CENTER);

                    //Process AmpYear Subsystems
                    for (int i = LoadGlobals.SubsystemArrayCache.Length - 1; i >= 0; --i)
                    {
                        PwrPartList partFnd;
                        string keyValue = AYVesselPartLists.CreatePartKey(partId, SubsystemName(LoadGlobals.SubsystemArrayCache[i]));
                        if (AYVesselPartLists.VesselConsPartsList.TryGetValue(keyValue, out partFnd))
                        {
                            if (partFnd.ValidprtEmergShutDn && partFnd.PrtEmergShutDnInclude)
                            {
                                if (Emergencypowerdownactivated) //Emergency ShutDown Procedures
                                {
                                    partFnd.PrtPreEmergShutDnStateActive = SubsystemEnabled(LoadGlobals.SubsystemArrayCache[i]);
                                    AYVesselPartLists.VesselConsPartsList[keyValue] = partFnd;
                                    if (partFnd.PrtActive && _espPriority == (ESPPriority)partFnd.PrtEmergShutDnPriority)
                                        SetSubsystemEnabled(LoadGlobals.SubsystemArrayCache[i], false);
                                }
                                if (Emergencypowerdownreset) //Emergency Reset Procedures
                                {
                                    if (partFnd.PrtPreEmergShutDnStateActive && !partFnd.PrtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                                    {
                                        if (LoadGlobals.SubsystemArrayCache[i] == Subsystem.RCS)
                                        {
                                            _reenableRcs = true;
                                        }
                                        else
                                        {
                                            if (LoadGlobals.SubsystemArrayCache[i] == Subsystem.SAS)
                                            {
                                                _reenableSas = true;
                                            }
                                            else
                                            {
                                                SetSubsystemEnabled(LoadGlobals.SubsystemArrayCache[i], true);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utilities.Log("Exception occurred during ESP EmergencyShutDownProcedureAmpYearSystems processing");
                    Utilities.Log("Exception: {0}", ex);
                }
            }
        }

        private string PPESPkeyValue;
        private PwrPartList PPESPpartFnd;
        private void ProcessPartEmergencyShutdownProcedures(Part currentPart, PartModule module, bool prtActive)
        {
            if (Utilities.GameModeisFlight && (Emergencypowerdownactivated || Emergencypowerdownreset))  //Emergency Procedures
            {
                //string keyValue = AYVesselPartLists.CreatePartKey(currentPart.flightID, module.moduleName);
                PPESPkeyValue = AYVesselPartLists.CreatePartKey(currentPart.craftID, module.moduleName);
                if (AYVesselPartLists.VesselProdPartsList.TryGetValue(PPESPkeyValue, out PPESPpartFnd))
                {
                    if (PPESPpartFnd.ValidprtEmergShutDn && PPESPpartFnd.PrtEmergShutDnInclude)
                    {
                        if (Emergencypowerdownactivated) //Emergency ShutDown Procedures
                        {
                            if (_espPriority == PPESPpartFnd.PrtEmergShutDnPriority && PriorityNotProcessed)
                            {
                                PPESPpartFnd.PrtPreEmergShutDnStateActive = prtActive;
                                AYVesselPartLists.VesselProdPartsList[PPESPkeyValue] = PPESPpartFnd;
                                ProcessPartModuleShutdown(currentPart, module, PPESPkeyValue, PPESPpartFnd, prtActive);
                            }
                        }
                        if (Emergencypowerdownreset) //Emergency Reset Procedures
                        {
                            if (_espPriority == PPESPpartFnd.PrtEmergShutDnPriority && PriorityNotProcessed)
                            {
                                ProcessPartModuleRestart(currentPart, module, PPESPpartFnd, prtActive);
                            }
                        }
                    }
                }
                else
                {
                    if (AYVesselPartLists.VesselConsPartsList.TryGetValue(PPESPkeyValue, out PPESPpartFnd))
                    {
                        if (PPESPpartFnd.ValidprtEmergShutDn && PPESPpartFnd.PrtEmergShutDnInclude)
                        {
                            if (Emergencypowerdownactivated) //Emergency ShutDown Procedures
                            {
                                if (_espPriority == PPESPpartFnd.PrtEmergShutDnPriority && PriorityNotProcessed)
                                {
                                    PPESPpartFnd.PrtPreEmergShutDnStateActive = prtActive;
                                    AYVesselPartLists.VesselConsPartsList[PPESPkeyValue] = PPESPpartFnd;
                                    ProcessPartModuleShutdown(currentPart, module, PPESPkeyValue, PPESPpartFnd, prtActive);
                                }
                            }
                            if (Emergencypowerdownreset) //Emergency Reset Procedures
                            {
                                if (_espPriority == PPESPpartFnd.PrtEmergShutDnPriority && PriorityNotProcessed)
                                {
                                    ProcessPartModuleRestart(currentPart, module, PPESPpartFnd, prtActive);
                                }
                            }
                        }
                    }
                    else
                    {
                        Utilities.Log(
                        "Failed to find {0} in Dictionary for EmergencyShutdown Processing", PPESPkeyValue);
                    }
                }
            }
        }

        private void ProcessPartModuleShutdown(Part currentPart, PartModule module, string keyValue, PwrPartList partFnd, bool prtActive)
        {
            switch (module.moduleName)
            {
                case "ModuleIONPoweredRCS":
                    if (prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ModuleRCS tmpIonPoweredRcs = (ModuleRCS)module;
                        tmpIonPoweredRcs.rcsEnabled = false;
                        ScreenMessages.PostScreenMessage(
                            "Electricity Levels Critical! Disabling ION RCS!", 5.0f,
                            ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Disabling ION RCS");
                    }
                    break;

                case "ModulePPTPoweredRCS":
                    if (prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ModuleRCS tmpIonPoweredRcs = (ModuleRCS)module;
                        tmpIonPoweredRcs.rcsEnabled = false;
                        ScreenMessages.PostScreenMessage(
                            "Electricity Levels Critical! Disabling PPT RCS!", 5.0f,
                            ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Disabling PPT RCS");
                    }
                    break;

                case "ModuleDeployableSolarPanel":
                    if (FlightGlobals.ActiveVessel.atmDensity < 0.2 ||
                                FlightGlobals.ActiveVessel.situation != Vessel.Situations.SUB_ORBITAL
                                || FlightGlobals.ActiveVessel.situation != Vessel.Situations.FLYING)
                    {
                        if (!prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                        {
                            ModuleDeployableSolarPanel tmpSol = (ModuleDeployableSolarPanel)module;
                            tmpSol.Extend();
                            ScreenMessages.PostScreenMessage(
                                "Electricity Levels Critical! Extending Solar Panels!", 5.0f,
                                ScreenMessageStyle.UPPER_LEFT);
                            Utilities.Log("Extending solar array");
                        }
                    }
                    else
                        ScreenMessages.PostScreenMessage(
                            "Electricity Levels Critical! In Atmosphere can not Extend Solar Panels!", 5.0f,
                            ScreenMessageStyle.UPPER_LEFT);
                    break;

                case "ModuleWheelMotor":
                    if (prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ModuleWheelMotor tmpWheel = (ModuleWheelMotor)module;
                        tmpWheel.motorEnabled = false;
                        ScreenMessages.PostScreenMessage(
                            "Electricity Levels Critical! Disabling Wheel Motors!", 5.0f,
                            ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Disabling Wheel motors");
                    }
                    break;
                    
                case "ModuleLight":
                    if (prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ModuleLight tmpLight = (ModuleLight)module;
                        tmpLight.LightsOff();
                        ScreenMessages.PostScreenMessage("Electricity Levels Critical! Turning off Lights!",
                            5.0f, ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Turning off lights");
                    }
                    break;

                case "ModuleDataTransmitter":
                    if (prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ModuleDataTransmitter tmpAnt = (ModuleDataTransmitter)module;
                        tmpAnt.StopTransmission();
                        ScreenMessages.PostScreenMessage("Electricity Levels Critical! Turning off Transmitters!", 5.0f, ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Turning off DataTransmitter");
                    }
                    break;

                case "ModuleReactionWheel":
                    if (prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ModuleReactionWheel tmpRw = (ModuleReactionWheel)module;
                        tmpRw.enabled = false;
                        ScreenMessages.PostScreenMessage("Electricity Levels Critical! Turning off ReactionWheel!", 5.0f, ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Turning off reactionwheel");
                    }
                    break;
                    
                case "ModuleResourceHarvester":
                    if (prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ModuleResourceHarvester tmpHvstr = (ModuleResourceHarvester)module;
                        tmpHvstr.DisableModule();
                        ScreenMessages.PostScreenMessage("Electricity Levels Critical! Turning off Harvester!", 5.0f, ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Turning off harvester");
                    }

                    break;

                case "ModuleResourceConverter":
                    if (prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ModuleResourceConverter tmpRegRc = (ModuleResourceConverter)module;
                        tmpRegRc.DisableModule();
                        ScreenMessages.PostScreenMessage("Electricity Levels Critical! Turning off Converter!", 5.0f, ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Turning off converter");
                    }

                    break;

                case "ModuleActiveRadiator":
                    if (prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ModuleActiveRadiator tmpRad = (ModuleActiveRadiator)module;
                        tmpRad.Shutdown();
                        ScreenMessages.PostScreenMessage("Electricity Levels Critical! Turning off Radiator!",
                            5.0f, ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Turning off Radiator");
                    }
                    break;

                case "ModuleEnviroSensor":
                    if (prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ModuleEnviroSensor tmpEnvS = (ModuleEnviroSensor)module;
                        if (tmpEnvS.sensorActive) tmpEnvS.Toggle();
                        ScreenMessages.PostScreenMessage("Electricity Levels Critical! Turning off EnviroSensor!",
                            5.0f, ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Turning off EnviroSensor");
                    }
                    break;

                case "ModuleNavLight":
                    if (prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ALWrapper.ALNavLight tmpLight = new ALWrapper.ALNavLight(currentPart);
                        tmpLight.navLightSwitch = 0;
                        ScreenMessages.PostScreenMessage("Electricity Levels Critical! Turning off NavLight!", 5.0f, ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Turning off navlight");
                    }
                    break;
                case "Curved Solar Panel":
                    if (!prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        if (FlightGlobals.ActiveVessel.atmDensity < 0.2 ||
                            FlightGlobals.ActiveVessel.situation != Vessel.Situations.SUB_ORBITAL
                            || FlightGlobals.ActiveVessel.situation != Vessel.Situations.FLYING)
                        {
                            NFSWrapper.NFSCurvedPanel tmpGen = new NFSWrapper.NFSCurvedPanel(currentPart);
                            tmpGen.Deploy();
                            ScreenMessages.PostScreenMessage(
                            "Electricity Levels Critical! Extending Panels!", 5.0f,
                            ScreenMessageStyle.UPPER_LEFT);
                            Utilities.Log("Extending Panels");
                        }
                    }
                    break;

                case "ModuleRTAntenna":
                    if (prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        RTWrapper.RTAntenna tmpAnt = new RTWrapper.RTAntenna(currentPart);
                        tmpAnt.Activated = false;
                        ScreenMessages.PostScreenMessage("Electricity Levels Critical! Turning off RTAntenna!", 5.0f, ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Turning off RTAntenna");
                    }
                    break;

                case "SCANsat":
                    if (prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ScanSatWrapper.SCANsat tmpSs = new ScanSatWrapper.SCANsat(currentPart);
                        tmpSs.stopScan();
                        ScreenMessages.PostScreenMessage("Electricity Levels Critical! Turning off SCANsat!", 5.0f, ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Turning off SCANsat");
                    }
                    break;

                case "TacGenericConverter":
                    if (prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        TACLSWrapper.TACLSGenericConverter tacGC = new TACLSWrapper.TACLSGenericConverter(currentPart);
                        tacGC.DeactivateConverter();
                        ScreenMessages.PostScreenMessage("Electricity Levels Critical! Turning off TACConverter!", 5.0f, ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Turning off TACConverter");
                    }
                    break;

                case "Scrubber":
                    if (prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {

                        BaseEventList scrubbereventlist = currentPart.Events;
                        scrubbereventlist["DeactivateEvent"].Invoke();
                        ScreenMessages.PostScreenMessage("Electricity Levels Critical! Turning off Scrubber!",
                            5.0f, ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Turning off Scrubber");
                    }
                    break;

                case "Greenhouse":
                    if (prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {

                        BaseFieldList ghfieldlist = currentPart.Fields;
                        ghfieldlist.SetValue("lamps", 0.0f);
                        ScreenMessages.PostScreenMessage("Electricity Levels Critical! Turning off Greenhouse!",
                            5.0f, ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Turning off Greenhouse");
                    }
                    break;
                case "GravityRing":
                    if (prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {

                        BaseFieldList grfieldlist = currentPart.Fields;
                        grfieldlist.SetValue("speed", 0.0f);
                        ScreenMessages.PostScreenMessage("Electricity Levels Critical! Turning off GravityRing!",
                            5.0f, ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Turning off GravityRing");
                    }
                    break;
                case "ModuleLimitedDataTransmitter":
                    if (prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        currentPart.deactivate();
                        ScreenMessages.PostScreenMessage("Electricity Levels Critical! Turning off ARAntenna!",
                            5.0f, ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Turning off ARAntenna");
                    }
                    break;
            }
        }

        private void ProcessPartModuleRestart(Part currentPart, PartModule module, PwrPartList partFnd, bool prtActive)
        {
            switch (module.moduleName)
            {
                case "ModuleIONPoweredRCS":
                    if (partFnd.PrtPreEmergShutDnStateActive && !prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ModuleRCS tmpIonPoweredRcs = (ModuleRCS)module;
                        tmpIonPoweredRcs.rcsEnabled = true;
                        ScreenMessages.PostScreenMessage(
                            "Electricity Levels OK Enable ION RCS!", 5.0f,
                            ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Enabling ION RCS");
                    }
                    break;

                case "ModulePPTPoweredRCS":
                    if (partFnd.PrtPreEmergShutDnStateActive && !prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ModuleRCS tmpIonPoweredRcs = (ModuleRCS)module;
                        tmpIonPoweredRcs.rcsEnabled = true;
                        ScreenMessages.PostScreenMessage(
                            "Electricity Levels OK Enable PPT RCS!", 5.0f,
                            ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Enabling PPT RCS");
                    }
                    break;

                case "ModuleDeployableSolarPanel":
                    ModuleDeployableSolarPanel tmpSol = (ModuleDeployableSolarPanel) module;
                    if (!partFnd.PrtPreEmergShutDnStateActive &&
                        tmpSol.deployState != ModuleDeployablePart.DeployState.RETRACTED && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        tmpSol.Retract();
                        ScreenMessages.PostScreenMessage(
                            "Electricity Levels OK Retracting Solar Panels!", 5.0f,
                            ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Retracting solar array");
                    }
                        
                    break;

                case "ModuleWheelMotor":
                    if (partFnd.PrtPreEmergShutDnStateActive && !prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ModuleWheelMotor tmpWheel = (ModuleWheelMotor)module;
                        tmpWheel.motorEnabled = true;
                            ScreenMessages.PostScreenMessage(
                                "Electricity Levels OK Enable Wheels!", 5.0f,
                                ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Enabling wheels");
                    }

                    break;

                case "ModuleLight":
                    if (partFnd.PrtPreEmergShutDnStateActive && !prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ModuleLight tmpLight = (ModuleLight)module;
                        tmpLight.LightsOn();
                        ScreenMessages.PostScreenMessage(
                            "Electricity Levels OK Enable Lights!", 5.0f,
                            ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Enabling lights");
                    }
                    break;

                case "ModuleDataTransmitter":
                    //Cannot re-activate a cancelled transmission, so cannot do anything here.
                    break;

                case "ModuleReactionWheel":
                    if (partFnd.PrtPreEmergShutDnStateActive && !prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ModuleReactionWheel tmpRw = (ModuleReactionWheel)module;
                        tmpRw.enabled = true;
                        ScreenMessages.PostScreenMessage(
                            "Electricity Levels OK Enable ReactionWheel!", 5.0f,
                            ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Enabling reactionwheel");
                    }
                    break;

                case "ModuleResourceHarvester":
                    if (partFnd.PrtPreEmergShutDnStateActive && !prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ModuleResourceHarvester tmpHvstr = (ModuleResourceHarvester)module;
                        tmpHvstr.EnableModule();
                        ScreenMessages.PostScreenMessage(
                            "Electricity Levels OK Enable Harvester!", 5.0f,
                            ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Enabling harvester");
                    }

                    break;

                case "ModuleResourceConverter":
                    if (partFnd.PrtPreEmergShutDnStateActive && !prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ModuleResourceConverter tmpRegRc = (ModuleResourceConverter)module;
                        tmpRegRc.EnableModule();
                        ScreenMessages.PostScreenMessage(
                            "Electricity Levels OK Enable Converter!", 5.0f,
                            ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Enabling converter");
                    }

                    break;

                case "ModuleActiveRadiator":
                    if (partFnd.PrtPreEmergShutDnStateActive && !prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ModuleActiveRadiator tmpRad = (ModuleActiveRadiator) module;
                        tmpRad.Activate();
                        ScreenMessages.PostScreenMessage("Electricity Levels OK Enable Radiator!",
                            5.0f, ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Enabling Radiator");
                    }
                    break;

                case "ModuleEnviroSensor":
                    if (partFnd.PrtPreEmergShutDnStateActive && !prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ModuleEnviroSensor tmpEnvS = (ModuleEnviroSensor)module;
                        if (!tmpEnvS.sensorActive) tmpEnvS.Toggle();
                        ScreenMessages.PostScreenMessage("Electricity Levels Critical! Enabling EnviroSensor!",
                            5.0f, ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Enabling EnviroSensor");
                    }
                    break;

                case "ModuleNavLight":
                    if (partFnd.PrtPreEmergShutDnStateActive && !prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ALWrapper.ALNavLight tmpLight = new ALWrapper.ALNavLight(currentPart);
                        tmpLight.navLightSwitch = 1;
                        ScreenMessages.PostScreenMessage(
                            "Electricity Levels OK Enable navLight!", 5.0f,
                            ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Enabling navlight");
                    }
                    break;

                case "Curved Solar Panel":
                    if (!partFnd.PrtPreEmergShutDnStateActive && prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        NFSWrapper.NFSCurvedPanel tmpGen = new NFSWrapper.NFSCurvedPanel(currentPart);
                        tmpGen.Retract();
                        ScreenMessages.PostScreenMessage(
                            "Electricity Levels OK Retracting Panels!", 5.0f,
                            ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Retracting panels");
                    }
                    break;

                case "ModuleRTAntenna":
                    if (partFnd.PrtPreEmergShutDnStateActive && !prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        RTWrapper.RTAntenna tmpAnt = new RTWrapper.RTAntenna(currentPart);
                        tmpAnt.Activated = true;
                        ScreenMessages.PostScreenMessage(
                            "Electricity Levels OK Enable RTAntenna!", 5.0f,
                            ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Enabling RTAntenna");
                    }
                    break;

                case "SCANsat":
                    if (partFnd.PrtPreEmergShutDnStateActive && !prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        ScanSatWrapper.SCANsat tmpSs = new ScanSatWrapper.SCANsat(currentPart);
                        tmpSs.startScan();
                        ScreenMessages.PostScreenMessage(
                            "Electricity Levels OK Enable SCANsat!", 5.0f,
                            ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Enabling SCANsat");
                    }
                    break;

                case "TacGenericConverter":
                    if (partFnd.PrtPreEmergShutDnStateActive && !prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        TACLSWrapper.TACLSGenericConverter tacGC = new TACLSWrapper.TACLSGenericConverter(currentPart);
                        tacGC.ActivateConverter();
                        ScreenMessages.PostScreenMessage(
                            "Electricity Levels OK Enable TACConverter!", 5.0f,
                            ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Enabling TACConverter");
                    }
                    break;

                case "ModuleLimitedDataTransmitter":
                    if (partFnd.PrtPreEmergShutDnStateActive && !prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {
                        currentPart.force_activate();
                        ScreenMessages.PostScreenMessage(
                            "Electricity Levels OK Enable ARAntenna!", 5.0f,
                            ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Enabling ARAntenna");
                    }
                    break;
                case "Scrubber":
                    if (partFnd.PrtPreEmergShutDnStateActive && !prtActive && _espPriority == partFnd.PrtEmergShutDnPriority)
                    {

                        BaseEventList gheventlist = currentPart.Events;
                        gheventlist["ActivateEvent"].Invoke();
                        ScreenMessages.PostScreenMessage("Electricity Levels OK Enable Scrubber!",
                            5.0f, ScreenMessageStyle.UPPER_LEFT);
                        Utilities.Log("Enabling Scrubber");
                    }
                    break;
                default:
                    break;
            }
        }

        private bool PNPreturnval;
        /// <summary>
        /// REturns TRUE is the current ESPPriorityProcessed bool relevent for the current _espPriority is FALSE.
        /// </summary>
        private bool PriorityNotProcessed
        {
            get
            {
                PNPreturnval = true;
                switch (_espPriority)
                {
                    case ESPPriority.HIGH:
                        if (ESPPriorityHighProcessed) PNPreturnval = false;
                        
                        break;
                    case ESPPriority.MEDIUM:
                        if (ESPPriorityMediumProcessed) PNPreturnval = false;
                        break;
                    case ESPPriority.LOW:
                        if (ESPPriorityLowProcessed) PNPreturnval = false;
                        break;
                }
                return PNPreturnval;
            }
        }

        private uint CalcAmpYearMgrPartIDpartId = 1111;

        private uint CalcAmpYearMgrPartID()
        {
            CalcAmpYearMgrPartIDpartId = 1111;
            if (rootPartID != 1111)  //If rootPartID is not the default value we have a rootPart to use.
            {
                return rootPartID;
            }
            
            //Root Part not set, so use first part in vessel.
            if (Utilities.GameModeisFlight)
            {
                if (FlightGlobals.ActiveVessel != null)
                    CalcAmpYearMgrPartIDpartId = FlightGlobals.ActiveVessel.rootPart.craftID;
            }
            else
            {
                if (Utilities.GameModeisEditor && EditorLogic.RootPart != null)
                    rootPartID = EditorLogic.RootPart.craftID;
            }
            return CalcAmpYearMgrPartIDpartId;
        }

        #endregion EmergencyShutdownProcedures
    }
}