using System;

namespace EasyTools.Events {
	public class EasyFunc<TResult> : EasyFuncBase<Func<TResult>> {
		public void InvokeAll() {
			foreach (var func in GetInvokeList()) func?.Invoke();
		}
	}

	public class EasyFunc<T, TResult> : EasyFuncBase<Func<T, TResult>> {
		public void InvokeAll(T arg) {
			foreach (var func in GetInvokeList()) func?.Invoke(arg);
		}
	}

	public class EasyFunc<T1, T2, TResult> : EasyFuncBase<Func<T1, T2, TResult>> {
		public void InvokeAll(T1 arg1, T2 arg2) {
			foreach (var func in GetInvokeList()) func?.Invoke(arg1, arg2);
		}
	}

	public class EasyFunc<T1, T2, T3, TResult> : EasyFuncBase<Func<T1, T2, T3, TResult>> {
		public void InvokeAll(T1 arg1, T2 arg2, T3 arg3) {
			foreach (var func in GetInvokeList()) func?.Invoke(arg1, arg2, arg3);
		}
	}

	public class EasyFunc<T1, T2, T3, T4, TResult> : EasyFuncBase<Func<T1, T2, T3, T4, TResult>> {
		public void InvokeAll(T1 arg1, T2 arg2, T3 arg3, T4 arg4) {
			foreach (var func in GetInvokeList()) func?.Invoke(arg1, arg2, arg3, arg4);
		}
	}

	public class EasyFunc<T1, T2, T3, T4, T5, TResult> : EasyFuncBase<Func<T1, T2, T3, T4, T5, TResult>> {
		public void InvokeAll(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) {
			foreach (var func in GetInvokeList()) func?.Invoke(arg1, arg2, arg3, arg4, arg5);
		}
	}

	public class EasyFunc<T1, T2, T3, T4, T5, T6, TResult> : EasyFuncBase<Func<T1, T2, T3, T4, T5, T6, TResult>> {
		public void InvokeAll(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) {
			foreach (var func in GetInvokeList()) func?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
		}
	}
}
