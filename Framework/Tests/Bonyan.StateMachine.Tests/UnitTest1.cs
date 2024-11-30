using System;
using Xunit;

namespace Bonyan.StateMachine.Tests
{
    public class TestStateMachineTests
    {
        [Fact]
        public async Task TestStateMachine_InitialState_ShouldBeEmpty()
        {
            var stateMachine = new TestStateMachine();
            var instance = new TestStateMachine.TestInstance { Id = Guid.NewGuid().ToString() };

            await stateMachine.ProcessAsync(instance,new TestStateMachine.RegisterEvent());
            Console.WriteLine($"Current State: {instance.State}");

            await stateMachine.ProcessAsync( instance,new TestStateMachine.ConfirmEvent());
            Console.WriteLine($"Current State: {instance.State}");

            
        }


    }
}
