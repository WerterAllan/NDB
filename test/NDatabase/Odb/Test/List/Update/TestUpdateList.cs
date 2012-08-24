using System.Collections;
using NDatabase.Odb;
using NUnit.Framework;

namespace Test.Odb.Test.List.Update
{
    [TestFixture]
    public class TestUpdateList : ODBTest
    {
        [Test]
        public virtual void Test1()
        {
            var file = "testeul.neodatis";
            DeleteBase(file);
            var dadosUsuario = new DadosUsuario();
            dadosUsuario.SetNome("Olivier");
            dadosUsuario.SetLogin("olivier");
            dadosUsuario.SetEmail("olivier@neodatis.org");
            dadosUsuario.SetOid("oid");
            IList l = new ArrayList();
            l.Add(new Publicacao("p1", "Texto 1"));
            dadosUsuario.SetPublicados(l);
            IOdb odb = null;
            try
            {
                odb = Open(file);
                odb.Store(dadosUsuario);
            }
            finally
            {
                if (odb != null)
                    odb.Close();
            }
            try
            {
                odb = Open(file);
                var l2 = odb.GetObjects<DadosUsuario>();
                Println(l2);
                var du = l2.GetFirst();
                du.GetPublicados().Add(new Publicacao("p2", "Texto2"));
                odb.Store(du);
            }
            finally
            {
                if (odb != null)
                    odb.Close();
            }
            try
            {
                odb = Open(file);
                var l2 = odb.GetObjects<DadosUsuario>();
                Println(l2);
                var du = l2.GetFirst();
                Println(du.GetPublicados());
                AssertEquals(2, du.GetPublicados().Count);
            }
            finally
            {
                if (odb != null)
                    odb.Close();
            }
        }

        [Test]
        public virtual void Test2()
        {
            var file = "testeul.neodatis";
            DeleteBase(file);
            var dadosUsuario = new DadosUsuario();
            dadosUsuario.SetNome("Olivier");
            dadosUsuario.SetLogin("olivier");
            dadosUsuario.SetEmail("olivier@neodatis.org");
            dadosUsuario.SetOid("oid");
            IList l = new ArrayList();
            l.Add(new Publicacao("p0", "Texto0"));
            dadosUsuario.SetPublicados(l);
            IOdb odb = null;
            try
            {
                odb = Open(file);
                odb.Store(dadosUsuario);
            }
            finally
            {
                if (odb != null)
                    odb.Close();
            }
            var size = 100;
            for (var i = 0; i < size; i++)
            {
                try
                {
                    odb = Open(file);
                    var l2 = odb.GetObjects<DadosUsuario>();
                    // println(l2);
                    var du = l2.GetFirst();
                    du.GetPublicados().Add(new Publicacao("p" + (i + 1), "Texto" + (i + 1)));
                    odb.Store(du);
                }
                finally
                {
                    if (odb != null)
                        odb.Close();
                }
            }
            try
            {
                odb = Open(file);
                var l2 = odb.GetObjects<DadosUsuario>();
                Println(l2);
                var du = l2.GetFirst();
                Println(du.GetPublicados());
                AssertEquals(size + 1, du.GetPublicados().Count);
                for (var i = 0; i < size + 1; i++)
                {
                    var p = (Publicacao) du.GetPublicados()[i];
                    AssertEquals("Texto" + (i), p.GetTexto());
                    AssertEquals("p" + (i), p.GetName());
                }
            }
            finally
            {
                if (odb != null)
                    odb.Close();
            }
        }
    }
}
