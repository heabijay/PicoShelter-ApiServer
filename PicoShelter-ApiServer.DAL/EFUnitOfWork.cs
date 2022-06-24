using PicoShelter_ApiServer.DAL.EF;
using PicoShelter_ApiServer.DAL.Entities;
using PicoShelter_ApiServer.DAL.Interfaces;
using PicoShelter_ApiServer.DAL.Repositories;
using System;

namespace PicoShelter_ApiServer.DAL
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext _db;

        public EFUnitOfWork(string connectionString)
        {
            _db = new(connectionString);
        }

        private AccountsRepository _accountRepository;
        private AlbumImagesRepository _albumImagesRepository;
        private AlbumsRepository _albumRepository;
        private ImagesRepository _imageRepository;
        private ProfilesRepository _profileRepository;
        private ProfileAlbumsRepository _profileAlbumsRepository;
        private RolesRepository _roleRepository;
        private ConfirmationsRepository _confirmationRepository;
        private ReportsRepository _reportsRepository;

        public IRepository<AccountEntity> Accounts => _accountRepository ??= new(_db);
        public IRepository<AlbumImageEntity> AlbumImages => _albumImagesRepository ??= new(_db);
        public IRepository<AlbumEntity> Albums => _albumRepository ??= new(_db);
        public IRepository<ImageEntity> Images => _imageRepository ??= new(_db);
        public IRepository<ProfileEntity> Profiles => _profileRepository ??= new(_db);
        public IRepository<ProfileAlbumEntity> ProfileAlbums => _profileAlbumsRepository ??= new(_db);
        public IRepository<RoleEntity> Roles => _roleRepository ??= new(_db);
        public IRepository<ConfirmationEntity> Confirmations => _confirmationRepository ??= new(_db);
        public IRepository<ReportEntity> Reports => _reportsRepository ??= new(_db);

        public void Save()
        {
            _db.SaveChanges();
        }

        private bool disposed = false;
        public void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }

                disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
