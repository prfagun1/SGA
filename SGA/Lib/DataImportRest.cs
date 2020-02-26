using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SGA.Interfaces;
using SGA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SGA.Lib
{
    public class DataImportRest : IDataImportRest
    {
        private readonly IUnitOfWork _iuw;
        private readonly string LogDescription = "ImportRest";


        public DataImportRest(IUnitOfWork iuw)
        {
            _iuw = iuw;
        }

        public int SetRestValue(ApplicationRest applicationRest, out string json)
        {
            string restValue = GetRestValue(applicationRest);
            json = restValue;
            try
            {
                var resultJson = JsonConvert.DeserializeObject<List<JObject>>(restValue);
                foreach (var line in resultJson)
                {
                    foreach (var data in line)
                    {
                        return int.TryParse(data.Value.ToString(), out int total) ? total : 0;
                    }

                }
            }
            catch
            {
                var resultJson = JsonConvert.DeserializeObject<JObject>(restValue);
                foreach (var data in resultJson)
                {
                    if (data.Value.ToString().ToUpper().Contains("ERRO"))
                    {
                        json = restValue;
                        return -1;
                    }
                }
            }

            return 0;
        }


        public bool ImportRestRescicoes(IQueryable<ApplicationRest> connectionRestIQueryable)
        {
            var applicationRestList = connectionRestIQueryable.Where(x => x.ApplicationTypeId == (int)EnumSGA.ConnectionType.ConsultaRescisoes).ToList();

            foreach (var applicationRest in applicationRestList)
            {
                try
                {
                    List<ApplicationRestResult> resultList = GetResultList(applicationRest);

                    foreach (var line in resultList)
                    {
                        //string username = line.Columns[11];
                        //string chaveRegistro = line.Columns[0];

                        string username = line.Columns[1];
                        string chaveRegistro = "";


                        //Caso o usuário já tenha sido importado, verifica se precisa atualizar a data de rescisão
                        var userHRRevision = _iuw.UserHRRepository.Get(x => x.Username == username);
                        if (userHRRevision != null)
                        {
                            DateTime metadadosDate = Convert.ToDateTime(line.Columns[7]);
                            if (userHRRevision.MetadadosDate != metadadosDate) { 
                                userHRRevision.MetadadosDate = metadadosDate;
                                userHRRevision.ChangeDate = DateTime.Now;

                                _iuw.UserHRRepository.Update(userHRRevision);
                                continue;
                            }
                        }

                        UserHR userHR = new UserHR();

                        var userDetails = _iuw.UserDetailsRepository.Get(x => x.Username == username);


                        if (userDetails != null)
                        {
                            userHR.Username = userDetails.Username;
                            userHR.FullName = userDetails.FullName;
                            userHR.JobRole = userDetails.JobRole;
                            userHR.Department = userDetails.Department;
                            userHR.CC = userDetails.CC;
                        }
                        else
                        {
                            userHR.Username = username;
                        }

                        userHR.ChangeDate = DateTime.Now;
                        userHR.StatusLocal = EnumSGA.UserHRStatusLocal.Pendente;
                        userHR.StatusRH = EnumSGA.UserHRStatusHR.Desligado;
                        //userHR.MetadadosDate = Convert.ToDateTime(line.Columns[5]);
                        userHR.MetadadosDate = Convert.ToDateTime(line.Columns[7]);
                        userHR.ChaveRegistro = chaveRegistro;
                        userHR.MetadadosStatus = false;

                        _iuw.UserHRRepository.Create(userHR);
                        
                    }
                    
                    _iuw.Save();
                    _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Dados da aplicação {applicationRest.Name} para o processo {applicationRest.ApplicationType.Name} foram salvos no banco.");
                    resultList.Clear();
                    return true;
                }
                catch (Exception e)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao salvar dados da aplicação {applicationRest.Name} para o processo {applicationRest.ApplicationType.Name}. " + e.Message);
                    return false;
                }
            }
            return true;

        }


        public void ImportRestConsultaUsuariosSistema(IQueryable<ApplicationRest> connectionRestIQueryable)
        {
            var applicationRestList = connectionRestIQueryable.Where(x => x.ApplicationTypeId == (int)EnumSGA.ConnectionType.ConsultaUsuariosSistema).ToList();
            //_iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Importados para memória lista de aplicações REST, quantidade: {applicationRestList.Count}.");

            using (var dataImportHelper = new DataImportHelper(_iuw))
            {
                int sizeUserDetails = dataImportHelper.GetUserDetailsDictionarySize() + 1;
                //_iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Importados para memória lista de usuários REST, quantidade: {applicationRestList.Count}.");

                int sizeGroupDetails = dataImportHelper.GetGroupDetailsDictionaryySize() + 1;
                //_iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Importados para memória lista de grupos, quantidade: {applicationRestList.Count}.");

                foreach (var applicationRest in applicationRestList)
                {
                    try
                    {
                        List<ApplicationRestResult> resultList = GetResultList(applicationRest);

                        foreach (var line in resultList)
                        {
                            //Os dados estão invertidos no retorno das APIs

                            string username = line.Columns[1];
                            string groups = line.Columns[0];


                            //Remover quando alterar a API do totvs para não retornar os grupos separados por ,
                            foreach (var group in groups.Split(','))
                            {
                                UserAccess userAccess = new UserAccess();

                                sizeUserDetails = dataImportHelper.GetDatabaseUserData(applicationRest.ApplicationId, sizeUserDetails, username, userAccess);
                                sizeGroupDetails = dataImportHelper.GetDatabaseUserAccessGroupData(applicationRest.ApplicationId, sizeGroupDetails, group, userAccess);

                                _iuw.UserAccessRepository.Create(userAccess);
                            }
                        }

                        resultList = null;
                        _iuw.Save();
                        _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Dados da aplicação {applicationRest.Name} para o processo {applicationRest.ApplicationType.Name} foram salvos no banco.");
                    }
                    catch (Exception e)
                    {
                        _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao salvar dados da aplicação {applicationRest.Name} para o processo {applicationRest.ApplicationType.Name}. " + e.Message);
                    }
                }
            }

        }

        public bool ImportRestConsultaFuncionariosAfastados(IQueryable<ApplicationRest> connectionRestIQueryable)
        {
            var applicationRestList = connectionRestIQueryable.Where(x => x.ApplicationTypeId == (int)EnumSGA.ConnectionType.ConsultaFuncionariosAfastados).ToList();

            foreach (var applicationRest in applicationRestList)
            {
                try
                {
                    List<ApplicationRestResult> resultList = GetResultList(applicationRest);

                    foreach (var line in resultList)
                    {
                        UserHR userHR = new UserHR();
                        DateTime metadadosDate = DateTime.Now;
                        DateTime metadadosDateReturn = Convert.ToDateTime("31/12/9999");

                        string chaveRegistro = line.Columns[0];
                        string username = line.Columns[12];

                        //Somente importa uma vez
                        var userHRTest = _iuw.UserHRRepository.Get(x => x.ChaveRegistro == chaveRegistro);
                        if (userHRTest != null)
                        {
                            continue;
                        }

                        try
                        {
                            metadadosDate = Convert.ToDateTime(line.Columns[5]);
                        }
                        catch { }
                        try
                        {
                            metadadosDateReturn = Convert.ToDateTime(line.Columns[6]);
                        }
                        catch { }

                        var userDetails = _iuw.UserDetailsRepository.Get(x => x.Username == username);



                        if (userDetails != null)
                        {
                            userHR.Username = userDetails.Username;
                            userHR.FullName = userDetails.FullName;
                            userHR.JobRole = userDetails.JobRole;
                            userHR.Department = userDetails.Department;
                            userHR.CC = userDetails.CC;
                        }
                        else
                        {
                            userHR.Username = username;
                        }


                        userHR.ChangeDate = DateTime.Now;
                        userHR.StatusLocal = EnumSGA.UserHRStatusLocal.Pendente;
                        userHR.StatusRH = EnumSGA.GetUserHRStatusHR(line.Columns[1]);
                        userHR.MetadadosDate = metadadosDate;
                        userHR.MetadadosDateReturn = metadadosDateReturn;
                        userHR.StatusRH = EnumSGA.UserHRStatusHR.Afastado;
                        userHR.ChaveRegistro = chaveRegistro;

                        _iuw.UserHRRepository.Create(userHR);

                    }

                    _iuw.Save();
                    _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Dados da aplicação {applicationRest.Name} para o processo {applicationRest.ApplicationType.Name} foram salvos no banco.");
                    resultList.Clear();
                    return true;

                }
                catch (Exception e)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao salvar dados da aplicação {applicationRest.Name} para o processo {applicationRest.ApplicationType.Name}. " + e.Message);
                    return false;
                }

            }
            return true;

        }

        public void ImportRestConsultaGruposPermissoes(IQueryable<ApplicationRest> connectionRestIQueryable)
        {
            var applicationRestList = connectionRestIQueryable.Where(x => x.ApplicationTypeId == (int)EnumSGA.ConnectionType.ConsultaGruposPermissoes).ToList();

            var dataImportHelper = new DataImportHelper(_iuw);

            int sizeUserDetails = dataImportHelper.GetUserDetailsDictionarySize() + 1;
            int sizeGroupDetails = dataImportHelper.GetGroupDetailsDictionaryySize() + 1;

            foreach (var applicationRest in applicationRestList)
            {
                try
                {
                    List<ApplicationRestResult> resultList = GetResultList(applicationRest);

                    foreach (var line in resultList)
                    {
                        //Os dados estão invertidos no retorno das APIs

                        string group = line.Columns[1];
                        string permission = line.Columns[0];

                        GroupAccess groupAccess = new GroupAccess();

                        sizeGroupDetails = dataImportHelper.GetDatabaseGroupDetailsData(applicationRest.ApplicationId, sizeGroupDetails, group, groupAccess);

                        groupAccess.Permission = permission;
                        _iuw.GroupAccessRepository.Create(groupAccess);

                    }

                    resultList = null;
                    _iuw.Save();
                    _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Dados da aplicação {applicationRest.Name} para o processo {applicationRest.ApplicationType.Name} foram salvos no banco.");
                }
                catch (Exception e)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao salvar dados da aplicação {applicationRest.Name} para o processo {applicationRest.ApplicationType.Name}. " + e.Message);
                }
            }
        }

        private List<ApplicationRestResult> GetResultList(ApplicationRest applicationRest)
        {
            string restValue = GetRestValue(applicationRest);

            List<ApplicationRestResult> resultList = new List<ApplicationRestResult>();
            var userListDynamic = JsonConvert.DeserializeObject<List<JObject>>(restValue);

            foreach (var user in userListDynamic)
            {
                ApplicationRestResult column = new ApplicationRestResult();
                foreach (var data in user)
                {
                    column.AddColumns(data.Value.ToString());
                }
                resultList.Add(column);
            }
            return resultList;
        }

        public string GetRestValue(ApplicationRest applicationRest)
        {

            HttpClient client = new HttpClient();
            HttpResponseMessage response = new HttpResponseMessage();

            client.BaseAddress = new Uri(applicationRest.URL);

            if (applicationRest.Header != null)
            {
                var headerList = applicationRest.Header.Split('\n');
                foreach (var header in headerList)
                {
                    var keyValue = header.Split(',');
                    client.DefaultRequestHeaders.Add(keyValue[0], keyValue[1]);
                }
            }

            if (applicationRest.MD5)
            {
                var username = Cipher.Decrypt(applicationRest.Username, applicationRest.ChangeDate.ToString());
                var password = Cipher.Decrypt(applicationRest.Password, applicationRest.ChangeDate.ToString());

                var hashMD5 = Cipher.GetMD5Hash(password + DateTime.Now.ToString("ddMMyyyyHHmm"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                        "Basic",
                        Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes($"{username}:{hashMD5}"))
                );
            }

            switch (applicationRest.RestType)
            {
                case EnumSGA.RestType.GET:
                    response = Task.Run(async () => await client.GetAsync(applicationRest.API)).Result;
                    break;
                case EnumSGA.RestType.POST:
                    var jsonString = applicationRest.Json.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty).Replace("\\", "\\\\");
                    var json = JsonConvert.SerializeObject(jsonString);
                    response = Task.Run(async () => await client.PostAsync(applicationRest.API, new StringContent(jsonString, Encoding.UTF8, "application/json"))).Result;
                    break;
            }

            return Task.Run(async () => await response.Content.ReadAsStringAsync()).Result;

        }

        public void ImportRestCentroDeCustos(IQueryable<ApplicationRest> connectionRestIQueryable)
        {
            var applicationRestList = connectionRestIQueryable.Where(x => x.ApplicationTypeId == (int)EnumSGA.ConnectionType.ConsultaCentroDeCustos).ToList();

            foreach (var applicationRest in applicationRestList)
            {
                try
                {
                    List<ApplicationRestResult> resultList = GetResultList(applicationRest);


                    foreach (var line in resultList)
                    {
                        CC cc = new CC();

                        cc.Id = int.Parse(line.Columns[0]);
                        cc.Username = line.Columns[1].ToLower();
                        cc.Description = line.Columns[2];
                        cc.Name = line.Columns[3];

                        _iuw.CCRepository.Create(cc);

                    }

                    resultList = null;
                    _iuw.Save();
                    _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Dados da aplicação {applicationRest.Name} para o processo {applicationRest.ApplicationType.Name} foram salvos no banco.");

                }
                catch (Exception e)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao salvar dados da aplicação {applicationRest.Name} para o processo {applicationRest.ApplicationType.Name}. " + e.Message);
                }
            }
        }

        public bool ImportRestNewEmployees(IQueryable<ApplicationRest> connectionRestIQueryable)
        {
            var applicationRestList = connectionRestIQueryable.Where(x => x.ApplicationTypeId == (int)EnumSGA.ConnectionType.ConsultaAdmissoess).ToList();

            foreach (var applicationRest in applicationRestList)
            {
                try
                {
                    List<ApplicationRestResult> resultList = GetResultList(applicationRest);


                    foreach (var line in resultList)
                    {
                        UserCreateEmployee employee = new UserCreateEmployee();

                        if(line.Columns[9].Contains("Barbara")){
                            var teste = "teste";
                        }

                        string employeeId = line.Columns[2]; ;

                        employee.ChaveRegistro = line.Columns[0];
                        employee.MetadadosStatus = false;
                        employee.FullName = line.Columns[9];
                        employee.EmployeeId = employeeId;
                        employee.MetadadosId = int.Parse(line.Columns[4]);
                        employee.Department = line.Columns[7];
                        employee.CC = int.Parse(line.Columns[6]);
                        employee.JobRole = line.Columns[14];
                        employee.Status = EnumSGA.UserCreateEmployeeStatus.Pendind;
                        employee.AdmissionDate = Convert.ToDateTime(line.Columns[5]);
                        employee.User = "Service";
                        employee.ChangeDate = DateTime.Now;

                        var employeeDatabase = _iuw.UserCreateEmployeeRepository.Get(x => x.EmployeeId == employee.EmployeeId);

                        if (employeeDatabase == null)
                        {
                            //Verifica se a matricula já não foi importada, pois a API retorna registros duplicados
                            var employeeSaved = _iuw.UserCreateEmployeeRepository.Get(x => x.EmployeeId == employeeId);
                            if (employeeSaved != null) {
                                employee.Status = EnumSGA.UserCreateEmployeeStatus.OK;
                            }

                            _iuw.UserCreateEmployeeRepository.Create(employee);
                            _iuw.Save();
                        }

                    }

                    resultList = null;
                    _iuw.LogCustomRepository.SaveLogApplicationMessage(LogDescription, $"Dados da aplicação {applicationRest.Name} para o processo {applicationRest.ApplicationType.Name} foram salvos no banco.");

                    return true;

                }
                catch (Exception e)
                {
                    _iuw.LogCustomRepository.SaveLogApplicationError(LogDescription, $"Erro ao salvar dados da aplicação {applicationRest.Name} para o processo {applicationRest.ApplicationType.Name}. " + e.Message);
                    return false;
                }
            }
            return false;
        }
    }
}