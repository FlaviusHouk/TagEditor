using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TagManager.Helpers
{
    interface ICluster<T>
    {
        int Count { get; set; }
        double CheckDistance(T obj);
        T Main { get; }
    }

    class ClusterAnalisis<T, C> where C : ICluster<T>
    {
        private IEnumerable<C> _clusters;
        private IEnumerable<T> _data;
        private ManualResetEvent _ev;
        private int _sharedCounter = 0;
        private object _sharedLocker = new object();
        private object _clusterLocker = new object();
        List<Task> _tasks;

        public ClusterAnalisis(IEnumerable<T> data, IEnumerable<C> clusters)
        {
            _data = data;
            _clusters = clusters;
            _ev = new ManualResetEvent(false);
        }

        public void RunAnalysis()
        {
            foreach (T item in _data)
            {
                var distances = _clusters.Select(cluster => cluster.CheckDistance(item)).ToList();
                _clusters.ElementAt(distances.IndexOf(distances.Min())).Count++;
            }
        }

        private void MultitaskAnalysis()
        {
            int index;
            lock (_sharedLocker)
            {
                index = _sharedCounter;
                _sharedCounter++;
            }

            while (index < _data.Count())
            {
                T pixel = _data.ElementAt(index);

                var distances = _clusters.Select(cluster => cluster.CheckDistance(pixel)).ToList();
                C c = _clusters.ElementAt(distances.IndexOf(distances.Min()));

                lock (_clusterLocker)
                {
                    c.Count++;
                }

                lock (_sharedLocker)
                {
                    index = _sharedCounter;
                    _sharedCounter++;
                }
            }

        }

        private int _done = 0;
        private object _doneLock = new object();
        public void AfterWork(Task task)
        {
            lock(_doneLock)
                _done++;

            if (_done == _tasks.Count)
            {
                _ev.Set();
                _done = 0;
            }
        }

        public void RunAnalysisAsync(int tasksCount)
        {
            _tasks = new List<Task>(tasksCount);
            
            for (int i = 0; i < tasksCount; i++)
            {
                _tasks.Add(Task.Run(new Action(MultitaskAnalysis)).ContinueWith(new Action<Task>(AfterWork)));
            }

            _ev.WaitOne();
        }
    }
}
