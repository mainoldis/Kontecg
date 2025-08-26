When you want to write a simple log in the client, you can use
console.log('...') API as you may already know. However, it's not supported by all
browsers and your script may break as a result. You must first check if
console is available. You may also want to write logs somewhere else.
You may evem want to write logs at some other level. Kontecg platform
defines these safe logging functions:

    kontecg.log.debug('...');
    kontecg.log.info('...');
    kontecg.log.warn('...');
    kontecg.log.error('...');
    kontecg.log.fatal('...');

You can change the log-level by setting the **kontecg.log.level** to one of the
kontecg.log.levels (ex: kontecg.log.levels.INFO does not write to the debug logs).
These functions write logs to the browser's console by default, but you can
override/extend this behavior if you need to.
