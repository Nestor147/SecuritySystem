using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi;
using SecuritySystem.Core.GetQueryFilter.Autorization;
using SecuritySystem.Core.Interfaces.Validators;
using SecuritySystem.Core.QueryFilters.Autorization;
using SecuritySystem.Infrastructure.Validators.Autorization;
using SecuritySystem.Infrastructure.Validators.Core;
using SecuritySystem.Infrastructure.Validators.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Infrastructure.Validators
{
    public class AuthorizationRepositoryValidator : IAuthorizationRepositoryValidator
    {
        #region Applications

        public ResponseModel ValidateCreateApplication(ApplicationQueryFilter applicationQueryFilter)
        {
            try
            {
                var validator = new InsertApplicationV2Validator();
                var validationResult = validator.Validate(applicationQueryFilter);
                return MainValidator.IterationValidationResult(validationResult);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public ResponseModel ValidateUpdateApplication(ApplicationQueryFilter applicationQueryFilter)
        {
            try
            {
                var validator = new UpdateApplicationV2Validator();
                var validationResult = validator.Validate(applicationQueryFilter);
                return MainValidator.IterationValidationResult(validationResult);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        #endregion

        #region Resources (Nodes / Pages)

        public ResponseModel ValidateCreateNode(ResourceQueryFilter resourceQueryFilter)
        {
            try
            {
                var validator = new InsertNodeV2Validator();
                var validationResult = validator.Validate(resourceQueryFilter);
                return MainValidator.IterationValidationResult(validationResult);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public ResponseModel ValidateCreatePage(ResourceQueryFilter resourceQueryFilter)
        {
            try
            {
                var validator = new InsertPageV2Validator();
                var validationResult = validator.Validate(resourceQueryFilter);
                return MainValidator.IterationValidationResult(validationResult);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public ResponseModel ValidateUpdatePage(ResourceQueryFilter resourceQueryFilter)
        {
            try
            {
                var validator = new UpdatePageV2Validator();
                var validationResult = validator.Validate(resourceQueryFilter);
                return MainValidator.IterationValidationResult(validationResult);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public ResponseModel ValidateUpdateNode(ResourceQueryFilter resourceQueryFilter)
        {
            try
            {
                var validator = new UpdateNodeV2Validator();
                var validationResult = validator.Validate(resourceQueryFilter);
                return MainValidator.IterationValidationResult(validationResult);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public ResponseModel ValidateGetResources(GetResourcesQueryFilter getResourcesQueryFilter)
        {
            try
            {
                var validator = new GetResourceV2Validator();
                var validationResult = validator.Validate(getResourcesQueryFilter);
                return MainValidator.IterationValidationResult(validationResult);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        #endregion

        #region Menu

        public ResponseModel ValidateInsertMenuV2(MenuV2QueryFilter menuQueryFilter)
        {
            try
            {
                var validator = new InsertMenuV2Validator();
                var validationResult = validator.Validate(menuQueryFilter);
                return MainValidator.IterationValidationResult(validationResult);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        #endregion

        #region Roles

        public ResponseModel ValidateCreateRole(RoleQueryFilter roleQueryFilter)
        {
            try
            {
                var validator = new InsertRoleV2Validator();
                var validationResult = validator.Validate(roleQueryFilter);
                return MainValidator.IterationValidationResult(validationResult);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public ResponseModel ValidateUpdateRole(RoleQueryFilter roleQueryFilter)
        {
            try
            {
                var validator = new UpdateRoleV2Validator();
                var validationResult = validator.Validate(roleQueryFilter);
                return MainValidator.IterationValidationResult(validationResult);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public ResponseModel ValidateGetRoles(GetRoleQueryFilter getRoleQueryFilter)
        {
            try
            {
                var validator = new GetRoleV2Validator();
                var validationResult = validator.Validate(getRoleQueryFilter);
                return MainValidator.IterationValidationResult(validationResult);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public ResponseModel ValidateGetRolesByUser(GetUsersByRoleQueryFilter getUsersByRoleQueryFilter)
        {
            try
            {
                var validator = new GetRoleByUserV2Validator();
                var validationResult = validator.Validate(getUsersByRoleQueryFilter);
                return MainValidator.IterationValidationResult(validationResult);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public ResponseModel ValidateGetUsersByRole(GetUsersByRoleQueryFilter getUsersByRoleQueryFilter)
        {
            try
            {
                var validator = new GetUsersByRoleV2Validator();
                var validationResult = validator.Validate(getUsersByRoleQueryFilter);
                return MainValidator.IterationValidationResult(validationResult);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public ResponseModel ValidateInsertUserRoles(InsertUserRoleQueryFilter queryFilter)
        {
            try
            {
                var validator = new InsertMultiRoleMultiUserValidator();
                var validationResult = validator.Validate(queryFilter);
                return MainValidator.IterationValidationResult(validationResult);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public ResponseModel ValidateGetMenuByUserRoles(GetMenuQueryFilter getMenuQueryFilter)
        {
            try
            {
                var validator = new GetUserRoleMenuV2Validator();
                var validationResult = validator.Validate(getMenuQueryFilter);
                return MainValidator.IterationValidationResult(validationResult);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public ResponseModel ValidateInsertRoleMenu(InsertRoleResourceQueryFilter menuQueryFilter)
        {
            try
            {
                var validator = new InsertRoleMenuV2Validator();
                var validationResult = validator.Validate(menuQueryFilter);
                return MainValidator.IterationValidationResult(validationResult);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public ResponseModel ValidateGetRoleMenuByApplication(GetRoleMenuQueryFilter menuQueryFilter)
        {
            try
            {
                var validator = new GetRoleMenuV2Validator();
                var validationResult = validator.Validate(menuQueryFilter);
                return MainValidator.IterationValidationResult(validationResult);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public ResponseModel ValidateGetMenuByRole(GetMenuQueryFilter getMenuQueryFilter)
        {
            try
            {
                var validator = new GetMenuByRoleV2Validator();
                var validationResult = validator.Validate(getMenuQueryFilter);
                return MainValidator.IterationValidationResult(validationResult);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public ResponseModel ValidateInsertRoleResource(InsertRoleResourceQueryFilter queryFilter)
        {
            try
            {
                var validator = new InsertRoleResourceValidator();
                var validationResult = validator.Validate(queryFilter);
                return MainValidator.IterationValidationResult(validationResult);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        #endregion
    }
}
