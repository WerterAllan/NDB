using NDatabase.Odb;
using NUnit.Framework;

namespace Test.Odb.Test.Rollback
{
	[TestFixture]
    public class TestRollBack : ODBTest
	{
		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test1()
		{
			DeleteBase("rollback.neodatis");
			IOdb odb = Open("rollback.neodatis", "u1", "p1");
			odb.Store(new VO.Login.Function("f1"));
			odb.Store(new VO.Login.Function("f2"));
			odb.Store(new VO.Login.Function("f3"));
			odb.Close();
			odb = Open("rollback.neodatis", "u1", "p1");
			odb.Store(new VO.Login.Function("f3"));
			odb.Rollback();
			odb.Close();
			odb = Open("rollback.neodatis", "u1", "p1");
			AssertEquals(3, odb.GetObjects<VO.Login.Function>().Count
				);
			odb.Close();
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test2()
		{
			DeleteBase("rollback.neodatis");
			IOdb odb = Open("rollback.neodatis", "u1", "p1");
			odb.Store(new VO.Login.Function("f1"));
			odb.Store(new VO.Login.Function("f2"));
			odb.Store(new VO.Login.Function("f3"));
			odb.Close();
			odb = Open("rollback.neodatis", "u1", "p1");
			odb.Store(new VO.Login.Function("f3"));
			odb.Rollback();
			// odb.close();
			try
			{
				AssertEquals(3, odb.GetObjects<VO.Login.Function>().Count
					);
			}
			catch (OdbRuntimeException e)
			{
				string s = e.ToString();
				AssertFalse(s.IndexOf("ODB session has been rollbacked") == -1);
			}
			odb.Close();
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test3RollbackOneStore()
		{
			DeleteBase("rollback.neodatis");
			IOdb odb = Open("rollback.neodatis", "u1", "p1");
			odb.Store(new VO.Login.Function("f1"));
			odb.Store(new VO.Login.Function("f2"));
			odb.Store(new VO.Login.Function("f3"));
			odb.Close();
			odb = Open("rollback.neodatis", "u1", "p1");
			odb.Store(new VO.Login.Function("f3"));
			odb.Rollback();
			odb.Close();
			odb = Open("rollback.neodatis", "u1", "p1");
			AssertEquals(3, odb.GetObjects<VO.Login.Function>().Count
				);
			odb.Close();
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test4RollbackXXXStores()
		{
			DeleteBase("rollback.neodatis");
			IOdb odb = Open("rollback.neodatis", "u1", "p1");
			odb.Store(new VO.Login.Function("f1"));
			odb.Store(new VO.Login.Function("f2"));
			odb.Store(new VO.Login.Function("f3"));
			odb.Close();
			odb = Open("rollback.neodatis", "u1", "p1");
			for (int i = 0; i < 500; i++)
			{
				odb.Store(new VO.Login.Function("f3 - " + i));
			}
			odb.Rollback();
			odb.Close();
			odb = Open("rollback.neodatis", "u1", "p1");
			AssertEquals(3, odb.GetObjects<VO.Login.Function>().Count
				);
			odb.Close();
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test5RollbackDelete()
		{
			DeleteBase("rollback.neodatis");
			IOdb odb = Open("rollback.neodatis", "u1", "p1");
			odb.Store(new VO.Login.Function("f1"));
			odb.Store(new VO.Login.Function("f2"));
			odb.Store(new VO.Login.Function("f3"));
			odb.Close();
			odb = Open("rollback.neodatis", "u1", "p1");
			IObjects<VO.Login.Function> objects = odb.GetObjects<VO.Login.Function>();
			while (objects.HasNext())
			{
				odb.Delete(objects.Next());
			}
			odb.Rollback();
			odb.Close();
			odb = Open("rollback.neodatis", "u1", "p1");
			AssertEquals(3, odb.GetObjects<VO.Login.Function>().Count
				);
			odb.Close();
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test6RollbackDeleteAndStore()
		{
			DeleteBase("rollback.neodatis");
			IOdb odb = Open("rollback.neodatis", "u1", "p1");
			odb.Store(new VO.Login.Function("f1"));
			odb.Store(new VO.Login.Function("f2"));
			odb.Store(new VO.Login.Function("f3"));
			odb.Close();
			odb = Open("rollback.neodatis", "u1", "p1");
			IObjects<VO.Login.Function> objects = odb.GetObjects<VO.Login.Function>();
			while (objects.HasNext())
			{
				odb.Delete(objects.Next());
			}
			for (int i = 0; i < 500; i++)
			{
				odb.Store(new VO.Login.Function("f3 - " + i));
			}
			odb.Rollback();
			odb.Close();
			odb = Open("rollback.neodatis", "u1", "p1");
			AssertEquals(3, odb.GetObjects<VO.Login.Function>().Count
				);
			odb.Close();
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test7Update()
		{
			DeleteBase("rollback.neodatis");
			IOdb odb = Open("rollback.neodatis", "u1", "p1");
			odb.Store(new VO.Login.Function("1function"));
			odb.Store(new VO.Login.Function("2function"));
			odb.Store(new VO.Login.Function("3function"));
			odb.Close();
			odb = Open("rollback.neodatis", "u1", "p1");
			IObjects<VO.Login.Function> objects = odb.GetObjects<VO.Login.Function>();
			while (objects.HasNext())
			{
				VO.Login.Function f = (VO.Login.Function)objects
					.Next();
				f.SetName(f.GetName().Substring(1));
				odb.Store(f);
			}
			odb.Rollback();
			odb.Close();
			odb = Open("rollback.neodatis", "u1", "p1");
			AssertEquals(3, odb.GetObjects<VO.Login.Function>().Count
				);
			odb.Close();
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test8RollbackDeleteAndStore()
		{
			DeleteBase("rollback.neodatis");
			IOdb odb = Open("rollback.neodatis", "u1", "p1");
			odb.Store(new VO.Login.Function("f1"));
			odb.Store(new VO.Login.Function("f2"));
			odb.Store(new VO.Login.Function("f3"));
			odb.Close();
			odb = Open("rollback.neodatis", "u1", "p1");
			IObjects<VO.Login.Function> objects = odb.GetObjects<VO.Login.Function>();
			while (objects.HasNext())
			{
				VO.Login.Function f = (VO.Login.Function)objects
					.Next();
				f.SetName(f.GetName().Substring(1));
				odb.Store(f);
			}
			objects.Reset();
			while (objects.HasNext())
			{
				odb.Delete(objects.Next());
			}
			for (int i = 0; i < 500; i++)
			{
				odb.Store(new VO.Login.Function("f3 - " + i));
			}
			odb.Rollback();
			odb.Close();
			odb = Open("rollback.neodatis", "u1", "p1");
			AssertEquals(3, odb.GetObjects<VO.Login.Function>().Count
				);
			odb.Close();
		}

		/// <exception cref="System.Exception"></exception>
		public override void TearDown()
		{
			DeleteBase("rollback.neodatis");
		}
	}
}
