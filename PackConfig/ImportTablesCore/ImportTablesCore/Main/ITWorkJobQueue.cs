using ExcelReader;
using ImportTables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImportTables
{
	public class ITWorkJobQueue
	{
		private ITWorker mgr;
		private List<ITWorkJob> jobLst = new List<ITWorkJob>();
		private Queue<string> workJobQueue = new Queue<string>();
		private Predicate<ITWorkJob> jobDoneFunc;
		public ITWorkJobQueue(ITWorker worker)
		{
			mgr = worker;
			jobDoneFunc = IsJobEnd;
			for (int i = 0; i < 4; i++)
			{
				jobLst.Add(new ITWorkJob(mgr));
			}
		}
		public bool IsDone
		{
			get
			{
				var isDone = jobLst.Find(jobDoneFunc) == null;
				return isDone;
			}
		}
		public void EnququeJob(List<string> job)
		{
			foreach (var item in job)
			{
				workJobQueue.Enqueue(item);
			}	
		}
		public void Update()
		{
			while (workJobQueue.Count > 0)
			{
				ITWorkJob freeJob = null;
				for (int i = 0; i < jobLst.Count; i++)
				{
					var job = jobLst[i];
					if (job.IsDone)
					{
						freeJob = job;
						break;
					}
				}
				if (freeJob != null)
				{
					var task = workJobQueue.Dequeue();
					IntternalImportOne(task, freeJob);
				}
			}
		}
		private void IntternalImportOne(string name, ITWorkJob free_job)
		{
			var fileInfo = mgr.GetFileInfo(name);
			var fileSize = fileInfo.Length * 1f / 1024 / 1024;
			var reader = mgr.GetReader(name);
			var lastChangeTimeStr = Utils.Utility.Prefers.GeString(ITConf.FILE_LAST_CHANGE_TIME_KEY + name, "");
			var currChangeTime = fileInfo.LastWriteTime.Ticks;

#if !DEBUG
			if (long.TryParse(lastChangeTimeStr, out long lastChangeTime))
			{
				if (lastChangeTime == currChangeTime)
				{
					return;
				}
			}
#endif
			free_job.Start(reader, name);
		}
		private bool IsJobEnd(ITWorkJob job)
		{
			return !job.IsDone;
		}
	}
}