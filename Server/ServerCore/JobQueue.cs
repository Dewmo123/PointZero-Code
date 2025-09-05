using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServerCore
{
	public interface IJobQueue
	{
		void Push(Action job);
	}

	public class JobQueue : IJobQueue
	{
		Queue<Action> _jobQueue = new Queue<Action>();
		object _lock = new object();
		bool _flush = false;

		public void Push(Action job)
		{
			bool flush = false;

			lock (_lock)
			{
				_jobQueue.Enqueue(job);
				if (_flush == false)
					flush = _flush = true;
			}

			if (flush)
				Flush();
		}

		void Flush()
		{
			while (true)
			{
				Action action = Pop();
				if (action == null)
					return;

				action.Invoke();
			}
		}

		Action Pop()
		{
			lock (_lock)
			{
				if (_jobQueue.Count == 0)
				{
					_flush = false;
					return null;
				}
				return _jobQueue.Dequeue();
			}
		}
	}
	public class LockFreeJobQueue : IJobQueue
	{
		private readonly ConcurrentQueue<Action> _queue = new();
		private int _flush = 0;

		public void Push(Action job)
		{
			_queue.Enqueue(job);

			if (Interlocked.CompareExchange(ref _flush, 1, 0) == 0)
			{
				Flush();
			}
		}

		public void Flush()
		{
			do
			{
				while (_queue.TryDequeue(out var job))
				{
					job.Invoke();
				}

				Interlocked.Exchange(ref _flush, 0);

			} while (_queue.Count > 0 && Interlocked.CompareExchange(ref _flush, 1, 0) == 0);
		}
	}
}

