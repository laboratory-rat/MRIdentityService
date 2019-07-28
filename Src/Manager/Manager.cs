using AutoMapper;
using DL;
using Infrastructure.Consts;
using Infrastructure.Entity.AppUser;
using Infrastructure.Model.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MRApiCommon.Exception;
using MRApiCommon.Infrastructure.Interface;
using MRApiCommon.Manager;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace BLL
{
    public abstract class Manager : MRTokenAuthManager
    {
        protected readonly ILogger _logger;
        protected readonly RepositoryRole _repositoryRole;
        protected readonly RepositoryUser _repositoryUser;
        protected readonly ManagerUser _managerUser;
        protected readonly IMapper _mapper;

        protected User _currentUserCache;
        protected User _currentUser
        {
            get
            {
                if (_currentUserCache == null && _isAuthorized)
                {
                    var userTask = _managerUser.FindByIdAsync(_userId);
                    userTask.Wait();
                    _currentUserCache = userTask?.Result ?? null;
                }
                return _currentUserCache;
            }
        }

        protected bool _isCurrentUserAdmin => 
            _userRoles != null 
            && _userRoles.Contains(AppRoles.ADMINISTRATOR.ToString());

        public Manager(IHttpContextAccessor httpContextAccessor,
                       ILoggerFactory logger,
                       RepositoryRole repositoryRole,
                       RepositoryUser repositoryUser,
                       ManagerUser managerUser,
                       IMapper mapper) : base(httpContextAccessor)
        {
            _logger = logger.CreateLogger(GetType());
            _repositoryRole = repositoryRole;
            _repositoryUser = repositoryUser;
            _managerUser = managerUser;
            _mapper = mapper;
        }

        #region transforms

        protected Expression<Func<TEntity, object>> _transformSort<TEntity>(SortModel model)
            where TEntity : class, IMREntity
        {
            if(typeof(TEntity).GetProperty(model.Field) == null)
            {
                throw new MRException<object>((int)ExceptionCode.BAD_MODEL, "Bad sorting field: " + model.Field);
            }

            return x => model.Field;
        }

        #endregion

        #region exceptions

        protected MRException<object> _eAccessDenied(Type type)
            => _eAccessDenied($"Access to {type.Name} denied");
        protected MRException<object> _eAccessDenied(string message)
            => throw new MRException<object>((int)ExceptionCode.ACCESS_DENIED, message);

        protected MRException<object> _eNotFound(string message)
            => throw new MRException<object>((int)ExceptionCode.NOT_FOUND, message);

        protected MRException<T> _eNotFound<T>(string message)
            where T : class, new()
            => throw new MRException<T>((int)ExceptionCode.NOT_FOUND, message);

        #endregion
    }
}
