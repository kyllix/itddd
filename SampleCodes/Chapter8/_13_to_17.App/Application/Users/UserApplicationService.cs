﻿using System.Linq;
using _13_to_17.App.Application.Users.Commons;
using _13_to_17.App.Application.Users.Delete;
using _13_to_17.App.Application.Users.Get;
using _13_to_17.App.Application.Users.GetAll;
using _13_to_17.App.Application.Users.Register;
using _13_to_17.App.Application.Users.Update;
using _13_to_17.App.Models.Users;

namespace _13_to_17.App.Application.Users
{
    public class UserApplicationService
    {
        private readonly IUserRepository userRepository;
        private readonly UserService userService;

        public UserApplicationService(IUserRepository userRepository, UserService userService)
        {
            this.userRepository = userRepository;
            this.userService = userService;
        }

        public UserGetResult Get(UserGetCommand command)
        {
            var id = new UserId(command.Id);
            var user = userRepository.Find(id);
            if (user == null)
            {
                throw new UserNotFoundException(id, "ユーザが見つかりませんでした。");
            }

            var data = new UserData(user);

            return new UserGetResult(data);
        }

        public UserGetAllResult GetAll()
        {
            var users = userRepository.FindAll();
            var userModels = users.Select(x => new UserData(x)).ToList();
            return new UserGetAllResult(userModels);
        }

        public void Register(UserRegisterCommand command)
        {
            var name = new UserName(command.Name);
            var user = new User(name);

            if (userService.Exists(user))
            {
                throw new CanNotRegisterUserException(user, "ユーザは既に存在しています。");
            }

            userRepository.Save(user);
        }

        public void Update(UserUpdateCommand command)
        {
            var id = new UserId(command.Id);
            var user = userRepository.Find(id);
            if (user == null)
            {
                throw new UserNotFoundException(id);
            }

            if (command.Name != null)
            {
                var name = new UserName(command.Name);
                user.ChangeName(name);

                if (userService.Exists(user))
                {
                    throw new CanNotRegisterUserException(user, "ユーザは既に存在しています。");
                }
            }

            userRepository.Save(user);
        }

        public void Delete(UserDeleteCommand command)
        {
            var id = new UserId(command.Id);
            var user = userRepository.Find(id);
            if (user == null)
            {
                return;
            }

            userRepository.Delete(user);
        }
    }
}
}
