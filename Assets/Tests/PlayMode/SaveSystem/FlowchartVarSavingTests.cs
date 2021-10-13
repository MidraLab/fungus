﻿using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Fungus;

namespace SaveSystemTests
{
    public abstract class FlowchartVarSavingTests: SaveSysPlayModeTest
    {
        protected override string PathToScene => "Prefabs/FlowchartSavingTests";

        #region Prep work

        public override void SetUp()
        {
            base.SetUp();
            allFlowcharts = GameObject.FindObjectsOfType<Flowchart>();
            variablesToEncode = GetVarsOfFlowchartNamed(VariableHolderName);
            GetSaverNeeded();
            PrepareExpectedResults();
            PrepareInvalidInputs();
        }

        protected Flowchart[] allFlowcharts;
        protected IList<Variable> variablesToEncode;

        protected virtual IList<Variable> GetVarsOfFlowchartNamed(string name)
        {
            GameObject flowchartGO = GameObject.Find(name);
            Flowchart flowchart = flowchartGO.GetComponent<Flowchart>();
            return flowchart.Variables;
        }

        protected abstract string VariableHolderName { get; }

        protected virtual void GetSaverNeeded()
        {
            GetEncoderHolder();
            varSaver = hasEncoders.GetComponent<VarSaver>();
        }

        protected virtual void GetEncoderHolder()
        {
            hasEncoders = GameObject.Find(encoderContainerGOName);
        }

        protected GameObject hasEncoders;
        protected string encoderContainerGOName = "FlowchartEncoders";
        protected VarSaver varSaver;

        protected virtual void PrepareExpectedResults()
        {
            ExpectedResults.Clear();

            foreach (var varEl in variablesToEncode)
            {
                var varAsString = varEl.GetValue().ToString();
                ExpectedResults.Add(varAsString);
            }
        }

        protected IList<string> ExpectedResults { get; } = new List<string>();

        protected abstract void PrepareInvalidInputs();

        protected IList<Variable> invalidInputs;

        #endregion

        #region Tests

        [Test]
        public virtual void EncodeVars_PassingSingles()
        {
            encodingResults = VarsEncodedWithMultipleEncodeCalls();

            AssertEncodingSuccess();
        }

        protected IList<ISaveUnit<VariableInfo>> encodingResults;

        protected virtual IList<ISaveUnit<VariableInfo>> VarsEncodedWithMultipleEncodeCalls()
        {
            IList<ISaveUnit<VariableInfo>> savedVars = new List<ISaveUnit<VariableInfo>>();

            foreach (var varEl in variablesToEncode)
            {
                var result = varSaver.CreateSaveFrom(varEl);
                savedVars.Add(result);
            }

            return savedVars;
        }

        protected virtual void AssertEncodingSuccess()
        {
            IList<string> whatWeGot = GetValuesIn(encodingResults);
            bool success = ExpectedResults.HasSameContentsInOrderAs(whatWeGot);
            Assert.IsTrue(success);
        }

        protected virtual IList<string> GetValuesIn(IList<ISaveUnit<VariableInfo>> saveUnits)
        {
            IList<string> results = new string[saveUnits.Count];

            for (int i = 0; i < saveUnits.Count; i++)
            {
                var currentUnit = saveUnits[i];
                var unitValue = currentUnit.Contents.Value;
                results[i] = unitValue;
            }

            return results;
        }


        [Test]
        public virtual void EncodeVars_PassingIList()
        {
            encodingResults = VarsEncodedWithOneEncodeCall();

            AssertEncodingSuccess();
        }

        protected virtual IList<ISaveUnit<VariableInfo>> VarsEncodedWithOneEncodeCall()
        {
            // One that involves passing multiple vars at once to the encoder
            return varSaver.CreateSavesFrom(variablesToEncode);
        }

        #endregion

    }
}
