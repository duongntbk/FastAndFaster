using System;
using System.Collections.Generic;
using System.Linq;

namespace FastAndFaster.Tests.DummyClasses
{
    public interface IInvocationTarget
    {
        void Action(string name, int age);

        int FuncHandleVal(int first, int second);

        IEnumerable<string> FuncHandleRef(List<string> names, string name);
    }

    public class InvocationTarget : IInvocationTarget
    {
        public int ParameterlessFuncReturnValData { get; set; }

        public string ParameterlessFuncReturnRefData { get; set; }

        public bool ParameterlessActionCalled { get; private set; } = false;

        public bool VirtualActionCalled { get; private set; } = false;

        public string VirtualFuncData { get; set; }

        public (string, int) ActionData { get; private set; }

        public bool OverloadActionCalled { get; private set; }

        public int ParameterlessFuncReturnVal() => ParameterlessFuncReturnValData;

        public string ParameterlessFuncReturnRef()
        {
            #pragma warning disable CS0219 // Make sure that the method is not optimized away
            var dummy = "dummy";
            #pragma warning restore CS0219

            return ParameterlessFuncReturnRefData;
        }

        public void ParameterlessAction() => ParameterlessActionCalled = true;

        public void Action(string name, int age) => ActionData = (name, age);

        public IEnumerable<string> FuncHandleRef(List<string> names, string name)
        {
            var newList = new List<string>(names)
            {
                name
            };
            return newList;
        }

        public int FuncHandleVal(int first, int second) => first + second;

        public virtual void VirtualAction(int dummy1, int dummy2) => VirtualActionCalled = true;

        public virtual string VirtualFunc() => VirtualFuncData;

        public void OverloadAction() => OverloadActionCalled = true;

        public void OverloadAction(string dummy) => throw new Exception("Called the wrong overload");

        public string OverloadFunc(string s) => s;

        public int OverloadFunc(int first, int second) => first + second;

        public int InterfaceInput(IEnumerable<string> names) => names.Count();

        public IEnumerable<int> InterfaceOutput(int n) => Enumerable.Range(0, n);

        private void PrivateAction(string s, int i) => throw new Exception("Should not be called");

        private int PrivateFunc() => throw new Exception("Should not be called");
    }

    public class ChildInvocationTarget : InvocationTarget
    {
        public bool ChildVirtualActionCalled { get; private set; } = false;

        public string ChildVirtualFuncData { get; set; }

        public override void VirtualAction(int dummy1, int dummy2) => ChildVirtualActionCalled = true;

        public override string VirtualFunc() => ChildVirtualFuncData;
    }

    public abstract class InvocationAbstractTarget
    {
        public abstract void ActionMultipleParams(string text, int number);

        public abstract string FuncMultipleParams(string text, int number);
    }

    public class InvocationConcreteTarget : InvocationAbstractTarget
    {
        public int ActionMultipleParamsCalled { get; set; } = -1;

        public override void ActionMultipleParams(string text, int number) =>
            ActionMultipleParamsCalled = text.Length + number;

        public override string FuncMultipleParams(string text, int number) => $"{text}:{number}";
    }
}
