import { Directive, Input } from '@angular/core';
import { FormControl, NgControl } from '@angular/forms';

@Directive({
  selector: '[disableControl]',
})
export class DisableControlDirective {
  constructor(private ngControl: NgControl) {}

  @Input() set disableControl(isNotEditedCity: boolean) {
    const control: FormControl = this.ngControl.control as FormControl;

    if (!control) {
      return;
    }

    if (isNotEditedCity) {
      control.disable();
      return;
    }

    control.enable();
  }
}
