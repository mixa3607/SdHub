//     This code was generated by a Reinforced.Typings tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.

import { CaptchaType } from './misc.models';
import { IUserModel } from './misc.models';

export interface ISendResetPasswordEmailRequest
{
	login: string;
	captchaType: CaptchaType;
	captchaCode: string;
}
export interface ISendResetPasswordEmailResponse
{
	success: boolean;
}
export interface IResetPasswordRequest
{
	login: string;
	code: string;
	newPassword: string;
}
export interface IResetPasswordResponse
{
	success: boolean;
}
export interface IConfirmEmailRequest
{
	code: string;
	login: string;
}
export interface IConfirmEmailResponse
{
	success: boolean;
}
export interface ILoginByPasswordRequest
{
	login: string;
	password: string;
	captchaType: CaptchaType;
	captchaCode: string;
}
export interface ILoginByRefreshTokenRequest
{
	refreshToken: string;
}
export interface ILoginResponse
{
	jwtToken: string;
	refreshToken: string;
	refreshTokenExpired: string;
	user: IUserModel;
}
export interface IRegisterRequest
{
	email: string;
	password: string;
	login: string;
	captchaType: CaptchaType;
	captchaCode: string;
}
export interface IRegisterResponse
{
	sendToEmail: string;
}
export interface IGetMeRequest
{
}
export interface IGetMeResponse
{
	user: IUserModel;
}
export interface ISendEmailConfirmationEmailRequest
{
	login: string;
	captchaType: CaptchaType;
	captchaCode: string;
}
export interface ISendEmailConfirmationEmailResponse
{
	success: boolean;
}
export interface IEditUserRequest
{
	login: string;
	about: string;
}
export interface IEditUserResponse
{
	user: IUserModel;
}
export interface IGetUserRequest
{
	login: string;
}
export interface IGetUserResponse
{
	user: IUserModel;
}
export abstract class UserRoleTypes
{
	public static User: string = `User`;
	public static Admin: string = `Admin`;
	public static HangfireRW: string = `HangfireRW`;
	public static HangfireRO: string = `HangfireRO`;
}
