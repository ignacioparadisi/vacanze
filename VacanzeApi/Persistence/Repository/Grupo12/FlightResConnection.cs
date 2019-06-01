using System;
using System.Collections.Generic;
using Npgsql;
using System.Data;
using vacanze_back.VacanzeApi.Common.Exceptions;
using vacanze_back.VacanzeApi.Common.Entities;
using vacanze_back.VacanzeApi.Common.Entities.Grupo12;
using vacanze_back.VacanzeApi.Common.Exceptions.Grupo12;

namespace vacanze_back.VacanzeApi.Persistence.Repository.Grupo12
{
    public class  FlightResConnection 
    {

        public void AddReservationFlight(Entity entity){
            Boolean tag=false;
            var resflight=(FlightRes) entity; 
            try{
                
                ValidateReservationFlight(resflight._id_fli,resflight._numPas);
             //   if(tag){
               /*     PgConnection.Instance.ExecuteFunction(
                    "AddReservationFlight(@seatNum,@tim,@numPas,@id_user,@pay,@id_fli)",
                    resflight._seatNum,resflight._timestamp,resflight._numPas,
                    resflight._id_user,resflight._id_pay,resflight._id_fli);*/
                 //   return true;
               // }else{
                   // return false;
               // }
                
            } catch(DBFailedException e){
            
                throw new DBFailedException("Tienes un error en la base de datos",e);
            
            }catch(InvalidStoredProcedureSignatureException){

                throw new InvalidStoredProcedureSignatureException("Tienes un error en el Stored Procedure");
            
            }
           
        }

        public List<FlightRes> GetReservationFlight(int id){
            var FlightList = new List<FlightRes>();
            var table = new DataTable();
           try{ 
                  
                table = PgConnection.Instance.ExecuteFunction("getReservationFlight(@rf_use_fk)",id);
                
                for (var i = 0; i < table.Rows.Count; i++){

                    int id_fli = Convert.ToInt32(table.Rows[i][0].ToString());
                    string seatNum = Convert.ToString(table.Rows[i][1].ToString());
                    string timeStam = Convert.ToString(table.Rows[i][2].ToString());
                    int numPas = Convert.ToInt32(table.Rows[i][3].ToString());
                    int rf_use = Convert.ToInt32(table.Rows[i][4].ToString());
                    int rf_pay = Convert.ToInt32(table.Rows[i][5].ToString());
                    int rf_fli = Convert.ToInt32(table.Rows[i][6].ToString());
                    var reservation_flight = new FlightRes(id_fli,seatNum,timeStam,numPas,rf_use,rf_pay,rf_fli);
                    FlightList.Add(reservation_flight);
                
                }
                return FlightList;

            }catch(DBFailedException e){

                throw new DBFailedException("Tienes un error en la base de datos",e);
            
            }catch(InvalidStoredProcedureSignatureException){

                throw new InvalidStoredProcedureSignatureException("Tienes un error en el Stored Procedure");
            
            }
            
        }

        public void DeleteReservationFlight(int id_reservation){
    
          try{
               
            var i=PgConnection.Instance.ExecuteFunction
            ("deletereservationflight(@id_reservation)",id_reservation);
            Console.WriteLine("--------------->",i);
            }catch(DBFailedException e){

                throw new DBFailedException("Tienes un error en la base de datos",e);
            
            }catch(InvalidStoredProcedureSignatureException){

                throw new InvalidStoredProcedureSignatureException("Tienes un error en el Stored Procedure");
            
            }
            
        }

        public string UpdateReservationFlightPay(int id_res,int id_pay){


                return "0";
        } 


        
        public string ShowReservationId(int id_res){
            

                return "0";
        } 


        public void ValidateReservationFlight(int id_flight,int num_pas){
            int cont=0;
            var res=new FlightRes();
            var table1= new DataTable();
            var table2= new DataTable();
            try{
                
                table1 = PgConnection.Instance.ExecuteFunction("GetCapacityFlight(@id_flight)",id_flight);
                table2 = PgConnection.Instance.ExecuteFunction("SumResFlight(@id_flight)",id_flight);
                res._num_capacity = Convert.ToInt32(table1.Rows[0][0].ToString());
                if(Convert.ToInt32(table2.Rows[0][0].ToString())==null){
                    res._sum_capacity=0;
                    Console.WriteLine(res._sum_capacity);
                };
                  Console.WriteLine(res._num_capacity);
                

               

            }catch(InvalidStoredProcedureSignatureException){

                throw new InvalidStoredProcedureSignatureException("Tienes un error en el Stored Procedure");
            
            }
           

        }





    }
}