namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using Maroontress.Oxbind.Impl;

public static class Validators
{
    public static Validator New<T>(Journal logger)
    {
        var nameBank = new QNameBank();
        return new Validator(typeof(T), logger, nameBank);
    }
}
