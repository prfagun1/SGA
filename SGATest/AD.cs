using Microsoft.EntityFrameworkCore;
using SGA.Interfaces;
using SGA.Lib;
using SGA.Models;
using SGA.Repositories;
using System;
using System.Linq;
using Xunit;

namespace SGATest
{
    public class AD
    {
        DbContextOptionsBuilder<SGAContext> optionsBuilder = new DbContextOptionsBuilder<SGAContext>();
        private IUnitOfWork _iuw;

        
        [Fact]
        public void TestaDesativacaoDeUsuario()
        {

            this.optionsBuilder = optionsBuilder.UseMySQL("server=localhost;port=3306;database=database;uid=username;password=password");
            using (var context = new SGAContext(optionsBuilder.Options)) {

                var _dbSet = context.Set<Ldap>();
                var ldap = _dbSet.Where(x => x.Id == 1).First();
                Assert.Equal(1, ldap.Id);
                
                using (var ad = new ADConnection(ldap)) {
                    ad.DisableUser("lpain");
                }
            }

            Assert.Equal(1, 1);
        }


        /*
    [Fact]
    public void TestaGeracaoUsuario() {
        this.optionsBuilder = optionsBuilder.UseMySQL("server=localhost;port=3306;database=database;uid=username;password=password");
        var context = new SGAContext(optionsBuilder.Options);
        _iuw = new UnitOfWork(context, null, null);

        UserHelper userHelper = new UserHelper(_iuw);
        string username = userHelper.GetLogin("Maria da Silva");

        Assert.Equal("msilva", username);
    }*/
        /*
            [Fact]
            public void VerificaUsuarioExisteMirth() {

                this.optionsBuilder = optionsBuilder.UseMySQL("server=localhost;port=3306;database=sgadb;uid=sga;password=90e83b5d0e6a3c9d5e414252cc2870a7");
                var context = new SGAContext(optionsBuilder.Options);

                _iuw = new UnitOfWork(context, null, null);
                DataImportRest dataImportRest = new DataImportRest(_iuw);


                UserHelper userHelper = new UserHelper(_iuw, dataImportRest);

                var teste = userHelper.GetUserExist("imp802111");

                Assert.Equal(1, 1);

            }
        */



    }
}
