using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;

namespace EasyTools {

	public static class UnityAsyncExtensions {

		public static TaskAwaiter<AsyncOperation> GetAwaiter(this AsyncOperation asyncOp) {
			TaskCompletionSource<AsyncOperation> tcs = new();
			asyncOp.completed += obj => tcs.SetResult(obj);
			return tcs.Task.GetAwaiter();
		}

		public static void WaitForComplete(this AsyncOperation asyncOp) {
			while (!asyncOp.isDone) Thread.Sleep(1);
		}

	}

}
