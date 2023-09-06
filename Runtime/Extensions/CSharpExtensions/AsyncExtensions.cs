using System;
using System.Threading.Tasks;

public static class AsyncExtensions {

	public static async void OnFinished<T>(this Task<T> task, Action<T> onFinished) {
		onFinished(await task);
	}
}
