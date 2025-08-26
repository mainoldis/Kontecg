Kontecg platform provides some common utility functions.

#### kontecg.utils.createNamespace

Used to create deep namespaces at once. Assume that we have a base 'kontecg'
namespace and want to create or get a 'kontecg.utils.strings.formatting'
namespace. Instead of this:

    //create or get namespace
    kontecg.utils = kontecg.utils || {};
    kontecg.utils.strings = kontecg.utils.strings || {};
    kontecg.utils.strings.formatting = kontecg.utils.strings.formatting || {};

    //add a function to the namespace
    kontecg.utils.strings.formatting.format = function() { ... };

We can write something like this:

    var formatting = kontecg.utils.createNamespace(kontecg, 'utils.strings.formatting';

    //Add a function to the namespace
    formatting.format = function() { ... };

This simplifies things by safely creating deep namespaces. Note that the first
argument is the root namespace that must exist.

#### kontecg.utils.formatString

Similar to string.Format in C\#. Example usage:

    var str = kontecg.utils.formatString('Hello {0}!', 'World'); //str = 'Hello World!'
    var str = kontecg.utils.formatString('{0} number is {1}.', 'Secret', 42); //str = 'Secret number is 42'
