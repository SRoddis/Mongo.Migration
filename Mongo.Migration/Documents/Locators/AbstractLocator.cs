using System.Collections.Generic;

namespace Mongo.Migration.Documents.Locators
{
    public abstract class AbstractLocator<TReturnType, TTypeIdentifier> : ILocator<TReturnType, TTypeIdentifier>
        where TReturnType : struct
        where TTypeIdentifier : class
    {
        private IDictionary<TTypeIdentifier, TReturnType> _locatesDictionary;

        protected IDictionary<TTypeIdentifier, TReturnType> LocatesDictionary
        {
            get
            {
                if (this._locatesDictionary == null)
                {
                    this.Locate();
                }

                return this._locatesDictionary;
            }

            set => this._locatesDictionary = value;
        }

        public abstract TReturnType? GetLocateOrNull(TTypeIdentifier identifier);

        public abstract void Locate();
    }
}