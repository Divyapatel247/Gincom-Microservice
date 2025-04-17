// src/app/auth-config.ts
import { AuthConfig } from 'angular-oauth2-oidc';

export const authConfig: AuthConfig = {
  issuer: 'http://localhost:5001',
  redirectUri: window.location.origin + '/index.html',
  clientId: 'client',
  responseType: 'code',
  scope: 'api.read api.write email profile offline_access', // Include offline_access for refresh tokens
  showDebugInformation: true,
  requireHttps: false, // For development
  useSilentRefresh: true,
  silentRefreshRedirectUri: window.location.origin + '/silent-refresh.html',
  timeoutFactor: 0.75, // Trigger refresh before token expires
  customQueryParams: { audience: 'product-service' } // Match API Gateway audience
};
