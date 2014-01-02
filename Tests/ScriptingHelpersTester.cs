namespace Tests
{
    using System;
    
    using NUnit.Framework;

    using DbUnicorn.DatabaseObjects;

    [TestFixture]
    public class ScriptingHelpersTester
    {
        private static object[] CanFindStoredProcedureNameForValidCreateScriptTestCases =
        {
            new object[] { "CREATE PROCEDURE ThisIsOnlyATest @parameter bit AS SELECT [@parameter] = @parameter;", Tuple.Create<int, int>(17, 15) },
	        new object[] { "CREATE PROCEDURE [ThisIsOnlyATest] @parameter bit AS SELECT [@parameter] = @parameter;", Tuple.Create<int, int>(17, 17) },
	        new object[] { "CREATE PROCEDURE \"ThisIsOnlyATest\" @parameter bit AS SELECT [@parameter] = @parameter;", Tuple.Create<int, int>(17, 17) },
	        new object[] { "CREATE PROCEDURE dbo.ThisIsOnlyATest @parameter bit AS SELECT [@parameter] = @parameter;", Tuple.Create<int, int>(17, 19) },
	        new object[] { "CREATE PROCEDURE dbo.[ThisIsOnlyATest] @parameter bit AS SELECT [@parameter] = @parameter;", Tuple.Create<int, int>(17, 21) },
	        new object[] { "CREATE PROCEDURE dbo.\"ThisIsOnlyATest\" @parameter bit AS SELECT [@parameter] = @parameter;", Tuple.Create<int, int>(17, 21) },
	        new object[] { "CREATE PROCEDURE [dbo].ThisIsOnlyATest @parameter bit AS SELECT [@parameter] = @parameter;", Tuple.Create<int, int>(17, 21) },
	        new object[] { "CREATE PROCEDURE \"dbo\".ThisIsOnlyATest @parameter bit AS SELECT [@parameter] = @parameter;", Tuple.Create<int, int>(17, 21) },
            new object[] { "CREATE PROCEDURE [dbo].[ThisIsOnlyATest] @parameter bit AS SELECT [@parameter] = @parameter;", Tuple.Create<int, int>(17, 23) },

            new object[] { "create PROCEDURE [dbo].[CONTACTGROUP_MEMBERS] @CONTACTGROUPID INT", Tuple.Create<int, int>(17, 28) }
        };

        [Test, TestCaseSource("CanFindStoredProcedureNameForValidCreateScriptTestCases")]
        public void CanFindStoredProcedureNameForValidCreateScript(string createScript, Tuple<int, int> expectedReturnValue)
        {
            Assert.AreEqual(expectedReturnValue, ScriptingHelpers.FindStoredProcedureNameInCreateScript(createScript));
        }
    }
}
