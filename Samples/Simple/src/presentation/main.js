
ipcDotNet.on('timer', (sender, argument) => {
    console.log("Timer %s", argument);
});

function showDeveloperTools() {
    ipcDotNet.send('showDeveloperTools', null, 
        (response) => { console.log(response); }, 
        (error, message) => { console.log("Error Code %d - %s", error, message); });
}