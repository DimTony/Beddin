using Beddin.Application.Common.DTOs;
using Beddin.Application.Common.Interfaces;
using Beddin.Application.Features.Users.Commands.RegisterUser;
using Beddin.Domain.Aggregates.Users;
using MediatR;

namespace Beddin.Application.Features.Users.Commands.CreateRole
{
    public sealed class CreateRoleHandler : IRequestHandler<CreateRoleCommand, ApiResponse<RoleDto>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateRoleHandler(
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var existingRole = await _roleRepository.GetByName(request.Name, cancellationToken);

            if (existingRole != null)
                return ApiResponse<RoleDto>.Fail("Role already exists.");

            var role = Role.Create(request.Name, request.Description);

            await _roleRepository.AddAsync(role, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var roleDto = new RoleDto(
                role.Id.Value,
                role.Name,
                role.Description,
                role.CreatedAt);

            return ApiResponse<RoleDto>.Ok(roleDto, "Role created!");


        }
    }
}
