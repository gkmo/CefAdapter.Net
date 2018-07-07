import { Component } from '@angular/core';

declare var ipcDotNet: any;

@Component({
  selector: 'app-counter-component',
  templateUrl: './counter.component.html'
})
export class CounterComponent {
  public currentCount = 0;

  public incrementCounter() {
    this.currentCount++;
  }

  public showDeveloperTools() {
    ipcDotNet.send('showDeveloperTools', null,
        (response) => { console.log(response); },
        (error, message) => { console.log('Error Code %d - %s', error, message); });
  }
}
