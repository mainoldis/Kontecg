The Message API is used to show a message to the user or to get a
confirmation from the user.

By default, the Message API is implemented using
[sweetalert](http://t4t5.github.io/sweetalert/). To make
sweetalert work, you should include its CSS & JavaScript files, then
include **kontecg.sweet-alert.js** to your page.

### Show message

Examples:

    kontecg.message.info('some info message', 'some optional title');
    kontecg.message.success('some success message', 'some optional title');
    kontecg.message.warn('some warning message', 'some optional title');
    kontecg.message.error('some error message', 'some optional title');

A success message is shown below:

<img src="../images/success_message.png" alt="Success message using sweetalert" class="img-thumbnail" />

### Confirmation

Example:

    kontecg.message.confirm(
        'User admin will be deleted.',
        'Are you sure?',
        function (isConfirmed) {
            if (isConfirmed) {
                //...delete user
            }
        }
    );

A confirmation message is shown below:

<img src="../images/confirmation_message.png" alt="Confirmation message using sweetalert" class="img-thumbnail" />

Kontecg platform internally uses the Message API. For example, it calls
kontecg.message.error if an [AJAX](/Pages/Documents/Javascript-API/AJAX)
call fails.
