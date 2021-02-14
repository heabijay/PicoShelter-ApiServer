using PicoShelter_ApiServer.DAL.Entities;
using System;

namespace PicoShelter_ApiServer.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public IRepository<AccountEntity> Accounts { get; }
        public IRepository<AlbumImageEntity> AlbumImages { get; }
        public IRepository<AlbumEntity> Albums { get; }
        public IRepository<ImageEntity> Images { get; }
        public IRepository<ProfileEntity> Profiles { get; }
        public IRepository<ProfileAlbumEntity> ProfileAlbums { get; }
        public IRepository<RoleEntity> Roles { get; }
        public IRepository<ConfirmationEntity> Confirmations { get; }

        public void Save();
    }
}
