using Microsoft.EntityFrameworkCore;
using PicoShelter_ApiServer.DAL.EF;
using PicoShelter_ApiServer.DAL.Entities;
using PicoShelter_ApiServer.DAL.Interfaces;
using PicoShelter_ApiServer.DAL.Repositories;
using System;

namespace PicoShelter_ApiServer.DAL
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private ApplicationContext db;
        public EFUnitOfWork(string connectionString)
        {
            db = new(connectionString);
        }

        private AccountsRepository accountRepository;
        private AlbumImagesRepository albumImagesRepository;
        private AlbumsRepository albumRepository;
        private ImagesRepository imageRepository;
        private ProfilesRepository profileRepository;
        private ProfileAlbumsRepository profileAlbumsRepository;
        private RolesRepository roleRepository;
        private ConfirmationsRepository confirmationRepository;

        public IRepository<AccountEntity> Accounts => accountRepository ??= new(db);
        public IRepository<AlbumImageEntity> AlbumImages => albumImagesRepository ??= new(db);
        public IRepository<AlbumEntity> Albums => albumRepository ??= new(db);
        public IRepository<ImageEntity> Images => imageRepository ??= new(db);
        public IRepository<ProfileEntity> Profiles => profileRepository ??= new(db);
        public IRepository<ProfileAlbumEntity> ProfileAlbums => profileAlbumsRepository ??= new(db);
        public IRepository<RoleEntity> Roles => roleRepository ??= new(db);
        public IRepository<ConfirmationEntity> Confirmations => confirmationRepository ??= new(db);

        public void Save()
        {
            db.SaveChanges();
        }

        private bool disposed = false;
        public void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    db.Dispose();
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
