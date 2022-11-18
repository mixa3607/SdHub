import {Component, OnInit} from '@angular/core';
import {AbstractControl, FormBuilder, FormControl, FormGroup, Validators} from "@angular/forms";
import {getErrorMessage} from "../../../shared/form-error-handling/handlers";
import {ToastrService} from "ngx-toastr";
import {HttpClient} from "@angular/common/http";
import {CustomValidators} from "apps/SdHub/src/app/shared/validators/customValidators";



@Component({
    selector: 'app-register-form',
    templateUrl: './register-form.component.html',
    styleUrls: ['./register-form.component.scss'],
})
export class RegisterFormComponent implements OnInit {
    public form: FormGroup;
    public hidePassword = true;

    constructor(private formBuilder: FormBuilder,
                private toastr: ToastrService,
                private http: HttpClient) {
        this.form = this.formBuilder.group({
            login: ['', Validators.compose([
                Validators.required,
                Validators.minLength(6),
                Validators.maxLength(30),
            ])],
            password: ['', Validators.compose([
                Validators.required,
                Validators.minLength(8),
                Validators.maxLength(100)
            ])],
            confirmPassword: ['', Validators.compose([
                Validators.required,
                Validators.minLength(8),
                Validators.maxLength(100)
            ])],
            recaptcha: [null, Validators.compose([
                Validators.required,
            ])],
        }, {validators: [CustomValidators.MatchingPasswords]});
    }

    ngOnInit(): void {
    }

    getErrorMessage(formControl: AbstractControl): string {
        return getErrorMessage(formControl);
    }
}
