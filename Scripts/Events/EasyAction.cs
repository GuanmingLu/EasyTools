using System;

namespace EasyTools.Events {
	public class EasyAction : EasyEventBase<Action> {
		public void Invoke() {
			foreach (var action in GetInvokeList()) action?.Invoke();
		}
	}

	public class EasyAction<T> : EasyEventBase<Action<T>> {
		public void Invoke(T arg) {
			foreach (var action in GetInvokeList()) action?.Invoke(arg);
		}
	}

	public class EasyAction<T1, T2> : EasyEventBase<Action<T1, T2>> {
		public void Invoke(T1 arg1, T2 arg2) {
			foreach (var action in GetInvokeList()) action?.Invoke(arg1, arg2);
		}
	}

	public class EasyAction<T1, T2, T3> : EasyEventBase<Action<T1, T2, T3>> {
		public void Invoke(T1 arg1, T2 arg2, T3 arg3) {
			foreach (var action in GetInvokeList()) action?.Invoke(arg1, arg2, arg3);
		}
	}

	public class EasyAction<T1, T2, T3, T4> : EasyEventBase<Action<T1, T2, T3, T4>> {
		public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4) {
			foreach (var action in GetInvokeList()) action?.Invoke(arg1, arg2, arg3, arg4);
		}
	}

	public class EasyAction<T1, T2, T3, T4, T5> : EasyEventBase<Action<T1, T2, T3, T4, T5>> {
		public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) {
			foreach (var action in GetInvokeList()) action?.Invoke(arg1, arg2, arg3, arg4, arg5);
		}
	}

	public class EasyAction<T1, T2, T3, T4, T5, T6> : EasyEventBase<Action<T1, T2, T3, T4, T5, T6>> {
		public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) {
			foreach (var action in GetInvokeList()) action?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
		}
	}
}
