using DeferredEvents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace DeferredEventsTests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void GettingDeferralCausesAwait()
        {
            var tsc = new TaskCompletionSource<bool>();

            var testClass = new TestClass();

            testClass.TestEvent += async (s, e) =>
            {
                var deferral = e.GetDeferral();

                await tsc.Task;

                deferral.Complete();
            };

            var handlersTask = testClass.RaiseTestEvent();

            Assert.IsFalse(handlersTask.IsCompleted);

            tsc.SetResult(true);

            Assert.IsTrue(handlersTask.IsCompleted);
        }

        [TestMethod]
        public void NotGettingDeferralCausesNoAwait()
        {
            var tsc = new TaskCompletionSource<bool>();

            var testClass = new TestClass();

            testClass.TestEvent += async (s, e) =>
            {
                await tsc.Task;
            };

            var handlersTask = testClass.RaiseTestEvent();

            Assert.IsTrue(handlersTask.IsCompleted);

            tsc.SetResult(true);
        }

        [TestMethod]
        public void UsingDeferralCausesAwait()
        {
            var tsc = new TaskCompletionSource<bool>();

            var testClass = new TestClass();

            testClass.TestEvent += async (s, e) =>
            {
                using (e.GetDeferral())
                {
                    await tsc.Task;
                }
            };

            var handlersTask = testClass.RaiseTestEvent();

            Assert.IsFalse(handlersTask.IsCompleted);

            tsc.SetResult(true);

            Assert.IsTrue(handlersTask.IsCompleted);
        }

        [DataTestMethod]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        public void MultipleHandlersCauseAwait(int firstToReleaseDeferral, int lastToReleaseDeferral)
        {
            var tsc = new[] {
                new TaskCompletionSource<bool>(),
                new TaskCompletionSource<bool>()
            };

            var testClass = new TestClass();

            testClass.TestEvent += async (s, e) =>
            {
                var deferral = e.GetDeferral();

                await tsc[0].Task;

                deferral.Complete();
            };

            testClass.TestEvent += async (s, e) =>
            {
                var deferral = e.GetDeferral();

                await tsc[1].Task;

                deferral.Complete();
            };

            var handlersTask = testClass.RaiseTestEvent();

            Assert.IsFalse(handlersTask.IsCompleted);

            tsc[firstToReleaseDeferral].SetResult(true);

            Assert.IsFalse(handlersTask.IsCompleted);

            tsc[lastToReleaseDeferral].SetResult(true);

            Assert.IsTrue(handlersTask.IsCompleted);
        }

        public class TestClass
        {
            public event EventHandler<DeferredEventArgs> TestEvent;

            public Task RaiseTestEvent() => TestEvent.InvokeAsync(this, new DeferredEventArgs());
        }
    }
}
