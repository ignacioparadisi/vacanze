using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using vacanze_back.VacanzeApi.Common.Entities.Grupo2;
using vacanze_back.VacanzeApi.Common.Exceptions;
using vacanze_back.VacanzeApi.Persistence.Repository.Grupo2;
using vacanze_back.VacanzeApi.Common.Entities;
using vacanze_back.VacanzeApi.LogicLayer.Command.Grupo2;
using vacanze_back.VacanzeApi.LogicLayer.Command;
using vacanze_back.VacanzeApi.LogicLayer.DTO.Grupo2;
using vacanze_back.VacanzeApi.LogicLayer.Mapper.Grupo2;
using Microsoft.Extensions.Logging;

namespace vacanze_back.VacanzeApi.Services.Controllers.Grupo2
{
     [Produces("application/json")]
     [Route("api/[controller]")]
     [EnableCors("MyPolicy")]
     [ApiController]
     public class UsersController : ControllerBase
     {
        private readonly ILogger _logger;
        // GET api/users
        /// <summary>
        /// Obtiene solo los usuarios empleados
        /// </summary>
        /// <returns>Devuelve una lista de usuarios que son empleados</returns>
        [HttpGet]
        public ActionResult<IEnumerable<UserDTO>> GetEmployees()
        {
            _logger.LogInformation("Entrando a GetEmployees()");
            List<UserDTO> usersdtos = new List<UserDTO>();
            List<User> users = new List<User>();
            try
            {
                _logger.LogInformation("Ejecutando Comando GetEmployeesCommand");
                GetEmployeesCommand command = CommandFactory.CreateGetEmployeesCommand();
                command.Execute();
                _logger.LogInformation("Comando GetEmployeesCommand Ejecutado");
                UserMapper mapper = new UserMapper();
                users = command.GetResult();
                usersdtos =  mapper.CreateDTOList(users);
                return Ok(usersdtos);
            }
            catch (InternalServerErrorException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(400);
            }
        }

        // GET api/users/5
        /// <summary>
        /// Busca un usuario con el id especificado
        /// </summary>
        /// <param name="id">Id del usuario que se desea buscar</param>
        /// <returns>Un usuario que corresponde al id especificado</returns>
        [HttpGet("{id}")]
        public ActionResult<UserDTO> Get(int id)
        {
            UserDTO userDTO = new UserDTO();
            User user;
            try
            {
                GetUserByIdCommand command = CommandFactory.CreateGetUserByIdCommand(id);
                command.Execute();
                user = command.GetResult();
                UserMapper mapper = new UserMapper();
                userDTO = mapper.CreateDTO(user);

            }
            catch (GeneralException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception)
            {
                return BadRequest("Error de servidor");
            }
            return Ok(userDTO);
        }

        // POST api/users
        /// <summary>
        /// Manda a guardar el usuario en la base de datos
        /// </summary>
        /// <param name="user">Usuario que se desea guardar</param>
        /// <returns>Retorna el usuario almacenado</returns>
        [HttpPost]
        public ActionResult<UserDTO> Post([FromBody] UserDTO user)
        {
            try
            {
                UserMapper mapper = new UserMapper();
                User entity = mapper.CreateEntity(user);
                PostUserCommand command = new PostUserCommand(entity);
                command.Execute();
                foreach (var role in user.Roles)
                {
                    PostUser_RoleCommand postRoleCommand = new PostUser_RoleCommand(entity.Id,role);
                    postRoleCommand.Execute();
                }
                user = mapper.CreateDTO((User)command.GetResult());
            }
            catch (GeneralException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception)
            {
                return BadRequest("Error agregando al usuario");
            }
            return Ok(user);
        }

        // PUT api/users/5
        /// <summary>
        /// Manda a actualizar el usuario en la base de datos
        /// </summary>
        /// <param name="id">Id del usuario que se desea actualizar</param>
        /// <param name="user">Información del usuario actualizada</param>
        /// <returns>Retorna el usuario actualizado</returns>
        [HttpPut("{id}")]
        // TODO: Retornar el usuario actualizado
        public ActionResult<int> Put(int id, [FromBody] User user)
        {
            int user_id;
            try
            {
                UpdateUserCommand updateUserCommand = new UpdateUserCommand(user,id);
                updateUserCommand.Execute();
                user_id = updateUserCommand.GetResult();
                DeleteUser_RoleCommand deleteUser_RoleCommand = new DeleteUser_RoleCommand(id);
                deleteUser_RoleCommand.Execute();
                PostUser_RoleCommand postUser_RoleCommand; 
                foreach (var role in user.Roles)
                {
                  //  UserRepository.AddUser_Role(id, role.Id);
                    postUser_RoleCommand = new PostUser_RoleCommand(id, role);
                    postUser_RoleCommand.Execute();
                }

                return Ok(user_id);

            }
            catch (GeneralException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception)
            {
                return BadRequest("Error actualizando al usuario");
            }
        }

        // DELETE api/users/1
        /// <summary>
        /// Manda a eliminar un usario por su id de la base de datos
        /// </summary>
        /// <param name="id">Id del usuario que se desea eliminar</param>
        /// <returns>Retorna el id del usuario que fue eliminado</returns>
        [HttpDelete("{id}")]
        public ActionResult<int> Delete(int id)
        {
            try
            {
                DeleteUserByIdCommand command = new DeleteUserByIdCommand(id);
                command.Execute();
            }
            catch (GeneralException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception)
            {
                return BadRequest("Error eliminando al usuario");
            }
            return Ok();
        }
    }
}