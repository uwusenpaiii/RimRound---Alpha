﻿using RimRound.Enums;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.UI
{
    public class MainTabWindow_RRGlobalSettings : MainTabWindow
    {
        const float windowWidth = 1200f;
        const float windowHeight = 900f;

        const float spaceBetweenCheckBoxes = 26;
        const float spaceBetweenNumberFields = 26;

        const float marginBetweenNumberFields = 8;

        const float bufferForCheckmarks = 40;
        const float bufferForNumberFields = 80;
        const float numberFieldRightOffset = 120;

        readonly int numberOfGizmoSettingCheckboxes = 5;
        readonly int numberOfExemptionSettingsCheckboxes = 6;

        static float _metaFloat;
        static string _metaStrBuffer;
        static float _metaFloat2;
        static string _metaStrBuffer2;
        static float _metaFloat3;
        static string _metaStrBuffer3;

        


        public override Vector2 RequestedTabSize
        {
            get
            {
                return new Vector2(windowWidth, windowHeight);
            }
        }



        public override void DoWindowContents(Rect inRect)
        {
            int numericFieldCount = 0;


            #region Title

            Text.Font = GameFont.Medium;
            Rect titleRect = new Rect(inRect.x, inRect.y, windowWidth, 2 * Text.LineHeight);

            if (Prefs.DevMode)
            {
                Rect metaRect = new Rect(titleRect.x + (inRect.width / 4), inRect.y, windowWidth / 5, Text.LineHeight);
                Widgets.TextFieldNumericLabeled<float>(metaRect, "Meta field 1 ", ref _metaFloat, ref _metaStrBuffer);

                Rect metaRect2 = new Rect(titleRect.x + (inRect.width / 2), inRect.y, windowWidth / 5, Text.LineHeight);
                Widgets.TextFieldNumericLabeled<float>(metaRect2, "Meta field 2 ", ref _metaFloat2, ref _metaStrBuffer2);

                Rect metaRect3 = new Rect(titleRect.x + (0.75f * inRect.width), inRect.y, windowWidth / 5, Text.LineHeight);
                Widgets.TextFieldNumericLabeled<float>(metaRect3, "Meta field 3 ", ref _metaFloat3, ref _metaStrBuffer3);
            }


            GUI.BeginGroup(titleRect);
            Widgets.Label(titleRect, "RR_Mtw_Title".Translate());

            GUI.EndGroup();

            #endregion

            #region Nutrition Settings Group
            Rect nutritionSettingsGroup = new Rect(inRect.x, titleRect.yMax, windowWidth / 3, 200);
            GUI.BeginGroup(nutritionSettingsGroup);

            //Category Title
            Text.Font = GameFont.Medium;
            Rect mapNutritionTitleRect = new Rect(0, 0, nutritionSettingsGroup.width, Text.LineHeight);
            Widgets.Label(mapNutritionTitleRect, "RR_Mtw_MapNutritionStatsTitle".Translate());

            float mapNutrition = Find.CurrentMap?.resourceCounter?.TotalHumanEdibleNutrition ?? 0;


            //Map Nutrition Section
            Text.Font = GameFont.Tiny;
            Rect rectMapNutContent = new Rect(0, mapNutritionTitleRect.yMax, nutritionSettingsGroup.width, 6 * Text.LineHeight);
            NutritionTable nutTable = Find.CurrentMap.resourceCounter.TotalHumanEdibleNutritionOfType();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("RR_Mtw_NutritionOverview_TotalNutrition".Translate() + ": " + mapNutrition.ToString("F1"));
            stringBuilder.AppendLine("RR_Mtw_NutritionOverview_SimpleMealNutrition".Translate() + ": " + nutTable.MealSimple.ToString("F1"));
            stringBuilder.AppendLine("RR_Mtw_NutritionOverview_FineMealNutrition".Translate() + ": " + nutTable.MealFine.ToString("F1"));
            stringBuilder.AppendLine("RR_Mtw_NutritionOverview_LavishMealNutrition".Translate() + ": " + nutTable.MealLavish.ToString("F1"));
            stringBuilder.AppendLine("RR_Mtw_NutritionOverview_UndesireableNutrition".Translate() + ": " + (nutTable.MealAwful + nutTable.RawBad + nutTable.DesperateOnly + nutTable.DesperateOnlyForHumanlikes).ToString("F1"));
            stringBuilder.AppendLine("RR_Mtw_NutritionOverview_OtherNutrition".Translate() + ": " +
                (mapNutrition -
                (nutTable.MealSimple +
                nutTable.MealFine +
                nutTable.MealLavish +
                nutTable.Undefined +
                nutTable.MealAwful +
                nutTable.RawBad +
                nutTable.DesperateOnly +
                nutTable.DesperateOnlyForHumanlikes)).ToString("F1"));

            Widgets.Label(rectMapNutContent, stringBuilder.ToString().Trim());

            Rect nutritionPerPawnLabel = new Rect(0, rectMapNutContent.yMax, nutritionSettingsGroup.width, Text.LineHeight);
            Widgets.Label(nutritionPerPawnLabel, "RR_Mtw_NutritionPerPawnLabel".Translate() + ": " + (mapNutrition / (Find.CurrentMap.mapPawns.ColonistCount + Find.CurrentMap.mapPawns.SlavesOfColonySpawned.Count)).ToString("F1"));

            GUI.EndGroup();
            #endregion

            #region Global Multipliers Group
            //Unbreak the spacing between the two groups.
            Rect globalMultipliersSettingsGroup = new Rect(nutritionSettingsGroup.x, nutritionSettingsGroup.yMax, nutritionSettingsGroup.width, windowHeight);

            GUI.BeginGroup(globalMultipliersSettingsGroup);

            //Category Title
            Text.Font = GameFont.Medium;
            Rect globalMultipliersSettingsTitleRect = new Rect(0, 0, globalMultipliersSettingsGroup.width, Text.LineHeight);
            Widgets.Label(globalMultipliersSettingsTitleRect, "RR_Mtw_GlobalMultipliersSettingsTitle".Translate());

            Rect globalMultipliersSettingsFieldRect = new Rect(0, globalMultipliersSettingsTitleRect.yMax, globalMultipliersSettingsTitleRect.width, 200);
            //globalMultipliersSettingsFieldRect.y += _metaFloat3;
            Text.Font = GameFont.Small;

            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.weightGainMultiplier, numericFieldCount++, "RR_Mtw_GlobalWeightGainMultiplierTitle");
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.weightLossMultiplier, numericFieldCount++, "RR_Mtw_GlobalWeightLossMultiplierTitle");
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.fullnessMultiplier, numericFieldCount++, "RR_Mtw_GlobalFullnessMultiplierTitle");
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.digestionRateMultiplier, numericFieldCount++, "RR_Mtw_GlobalDigestionRateMultiplierTitle");
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.stomachElasticityMultiplier, numericFieldCount++, "RR_Mtw_GlobalStomachElasticityMultiplierTitle");
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.ticksPerHungerCheck, numericFieldCount++, "RR_Mtw_TicksPerHungerCheckTitle");
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.ticksPerBodyChangeCheck, numericFieldCount++, "RR_Mtw_TicksPerBodyChangeCheckTitle");
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.softLimitMuliplier, numericFieldCount++, "RR_Mtw_GlobalSoftLimitMultiplier");
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.hardLimitMuliplier, numericFieldCount++, "RR_Mtw_GlobalHardLimitMultiplier");
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.weightToBeBed, numericFieldCount++, "RR_Mtw_GlobalBlobIntoBedThreshold");
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.weightToAdjustWiggleAngle, numericFieldCount++, "RR_Mtw_GlobalWeightToAdjustWiggleAngleThreshold");
            /**/
            globalMultipliersSettingsFieldRect.height = numericFieldCount * spaceBetweenNumberFields;

            GUI.EndGroup();

            #endregion

            #region Exemption Settings

            Rect exemptionSettingsGroup = new Rect(0.33333f * windowWidth, titleRect.yMax, 0.33333f * windowWidth, 200);
            GUI.BeginGroup(exemptionSettingsGroup);

            Text.Font = GameFont.Medium;
            Rect exemptionSettingsTitleRect = new Rect(0, 0, exemptionSettingsGroup.width, Text.LineHeight);
            Widgets.Label(exemptionSettingsTitleRect, "RR_Mtw_BodyChangeExemptionSettingsTitle".Translate());

            Text.Font = GameFont.Small;

            Rect exemptionSettingsCheckBoxesRect = new Rect(0, exemptionSettingsTitleRect.yMax, exemptionSettingsGroup.width, numberOfExemptionSettingsCheckboxes * spaceBetweenCheckBoxes);

            Functions.CheckboxLabeled(new Rect
            {
                x = 0,
                y = exemptionSettingsCheckBoxesRect.y,
                width = exemptionSettingsCheckBoxesRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
                "RR_Mtw_BodyChangeExemptionSettings_Male".Translate(), ref GlobalSettings.bodyChangeMale, false, null, null, false, () => { Functions.AssignBodyTypeCategoricalExemptions(true); });
            Functions.CheckboxLabeled(new Rect
            {
                x = 0,
                y = exemptionSettingsCheckBoxesRect.y + 1 * spaceBetweenCheckBoxes,
                width = exemptionSettingsCheckBoxesRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
                "RR_Mtw_BodyChangeExemptionSettings_Female".Translate(), ref GlobalSettings.bodyChangeFemale, false, null, null, false, () => { Functions.AssignBodyTypeCategoricalExemptions(true); });
            Functions.CheckboxLabeled(new Rect
            {
                x = 0,
                y = exemptionSettingsCheckBoxesRect.y + 2 * spaceBetweenCheckBoxes,
                width = exemptionSettingsCheckBoxesRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
                "RR_Mtw_BodyChangeExemptionSettings_HostileNPC".Translate(), ref GlobalSettings.bodyChangeHostileNPC, false, null, null, false, () => { Functions.AssignBodyTypeCategoricalExemptions(true); });
            Functions.CheckboxLabeled(new Rect
            {
                x = 0,
                y = exemptionSettingsCheckBoxesRect.y + 3 * spaceBetweenCheckBoxes,
                width = exemptionSettingsCheckBoxesRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
                "RR_Mtw_BodyChangeExemptionSettings_FriendlyNPC".Translate(), ref GlobalSettings.bodyChangeFriendlyNPC, false, null, null, false, () => { Functions.AssignBodyTypeCategoricalExemptions(true); });
            Functions.CheckboxLabeled(new Rect
            {
                x = 0,
                y = exemptionSettingsCheckBoxesRect.y + 4 * spaceBetweenCheckBoxes,
                width = exemptionSettingsCheckBoxesRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
                "RR_Mtw_BodyChangeExemptionSettings_Prisoner".Translate(), ref GlobalSettings.bodyChangePrisoners, false, null, null, false, () => { Functions.AssignBodyTypeCategoricalExemptions(true); });
            Functions.CheckboxLabeled(new Rect
            {
                x = 0,
                y = exemptionSettingsCheckBoxesRect.y + 5 * spaceBetweenCheckBoxes,
                width = exemptionSettingsCheckBoxesRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
                "RR_Mtw_BodyChangeExemptionSettings_Slave".Translate(), ref GlobalSettings.bodyChangeSlaves, false, null, null, false, () => { Functions.AssignBodyTypeCategoricalExemptions(true); });

            GUI.EndGroup();

            #endregion

            #region General Settings

            Rect generalSettingsRect = new Rect(exemptionSettingsGroup.x, exemptionSettingsGroup.yMax, exemptionSettingsGroup.width, exemptionSettingsGroup.height);
            GUI.BeginGroup(generalSettingsRect);

            Text.Font = GameFont.Medium;
            Rect generalSettingsTitleRect = new Rect(0, 0, exemptionSettingsGroup.width, Text.LineHeight);
            Widgets.Label(generalSettingsTitleRect, "RR_Mtw_GeneralSettings_Title".Translate());

            Text.Font = GameFont.Small;

            Rect generalSettingsCheckboxesRect = new Rect(0, generalSettingsTitleRect.yMax, generalSettingsRect.width, 200);

            Functions.CheckboxLabeled(new Rect
            {
                x = 0,
                y = generalSettingsTitleRect.yMax,
                width = generalSettingsRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
            "RR_Mtw_GeneralSettings_BurstingEnabled".Translate(), ref GlobalSettings.burstingEnabled);

            Functions.CheckboxLabeled(new Rect
            {
                x = 0,
                y = generalSettingsTitleRect.yMax + spaceBetweenCheckBoxes * 1,
                width = generalSettingsRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
            "RR_Mtw_GeneralSettings_ShowTattoosForCustomBodies".Translate(), ref GlobalSettings.showBodyTatoosForCustomSprites, false, null, null, false, () => { Functions.AssignBodyTypeCategoricalExemptions(true); });

            Functions.CheckboxLabeled(new Rect
            {
                x = 0,
                y = generalSettingsTitleRect.yMax + spaceBetweenCheckBoxes * 2,
                width = generalSettingsRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
            "RR_Mtw_GeneralSettings_PreferDefaultOverNaked".Translate(), ref GlobalSettings.preferDefaultOutfitOverNaked, false, null, null, false, () => { Functions.AssignBodyTypeCategoricalExemptions(true); });

            GUI.EndGroup();
            #endregion

            #region Gizmo Settings
            Rect gizmoSettingsGroup = new Rect(0.66666f * windowWidth, titleRect.yMax, 0.33333f * windowWidth, inRect.height - titleRect.height);
            GUI.BeginGroup(gizmoSettingsGroup);

            Text.Font = GameFont.Medium;
            Rect gizmoSettingsTitleRect = new Rect(0, 0, gizmoSettingsGroup.width, Text.LineHeight);
            Widgets.Label(gizmoSettingsTitleRect, "RR_Mtw_GizmoSettingsTitle".Translate());


            Rect gizmoSettingsCheckBoxesRect = new Rect(0, gizmoSettingsTitleRect.yMax, gizmoSettingsGroup.width, numberOfGizmoSettingCheckboxes * spaceBetweenCheckBoxes);

            Text.Font = GameFont.Small;


            Widgets.CheckboxLabeled(new Rect(
                gizmoSettingsCheckBoxesRect.x,
                gizmoSettingsCheckBoxesRect.y,
                gizmoSettingsCheckBoxesRect.width - bufferForCheckmarks,
                spaceBetweenCheckBoxes),
                "RR_Mtw_GizmoSettings_PawnDietManagementGizmo".Translate(), ref GlobalSettings.showPawnDietManagementGizmo);

            Widgets.CheckboxLabeled(new Rect
            {
                x = gizmoSettingsCheckBoxesRect.x,
                y = 1 * spaceBetweenCheckBoxes + gizmoSettingsCheckBoxesRect.y,
                width = gizmoSettingsCheckBoxesRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
                "RR_Mtw_GizmoSettings_SleepPostureManagementGizmo".Translate(), ref GlobalSettings.showSleepPostureManagementGizmo);
            Widgets.CheckboxLabeled(new Rect
            {
                x = gizmoSettingsCheckBoxesRect.x,
                y = 2 * spaceBetweenCheckBoxes + gizmoSettingsCheckBoxesRect.y,
                width = gizmoSettingsCheckBoxesRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
                "RR_Mtw_GizmoSettings_BlanketMangementGizmo".Translate(), ref GlobalSettings.showBlanketManagementGizmo);
            Widgets.CheckboxLabeled(new Rect
            {
                x = gizmoSettingsCheckBoxesRect.x,
                y = 3 * spaceBetweenCheckBoxes + gizmoSettingsCheckBoxesRect.y,
                width = gizmoSettingsCheckBoxesRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
                "RR_Mtw_GizmoSettings_ExemptionGizmo".Translate(), ref GlobalSettings.showExemptionGizmo);
            Widgets.CheckboxLabeled(new Rect
            {
                x = gizmoSettingsCheckBoxesRect.x,
                y = 4 * spaceBetweenCheckBoxes + gizmoSettingsCheckBoxesRect.y,
                width = gizmoSettingsCheckBoxesRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
                "RR_Mtw_GizmoSettings_BlobIntoBedGizmo".Translate(), ref GlobalSettings.showBlobIntobedGizmo);


            GUI.EndGroup();

            #endregion
        }

        public static void NumberFieldLabeledWithRect<T>(
            Rect categoryRect, ref NumericFieldData<T> numericFieldData, int positionNumberInList, string translationLabel , GameFont font = GameFont.Small) where T : struct
        {
            Text.Font = font;
            Rect boundingRect = new Rect(0, categoryRect.y + positionNumberInList * spaceBetweenNumberFields, categoryRect.width - numberFieldRightOffset, Text.LineHeight);
            Widgets.Label(boundingRect, translationLabel.Translate() + ": ");
            Widgets.TextFieldNumeric<T>(
                new Rect(boundingRect.xMax - bufferForNumberFields, boundingRect.y, bufferForNumberFields, Text.LineHeight),
                ref numericFieldData.threshold,
                ref numericFieldData.stringBuffer,
                numericFieldData.min,
                numericFieldData.max);
        }
    }
}