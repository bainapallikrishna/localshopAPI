// Auth Action Types
export const LOGIN_REQUEST = 'LOGIN_REQUEST';
export const LOGIN_SUCCESS = 'LOGIN_SUCCESS';
export const LOGIN_FAILURE = 'LOGIN_FAILURE';
export const LOGOUT = 'LOGOUT';
export const CLEAR_AUTH_ERROR = 'CLEAR_AUTH_ERROR';

// Register Action Types
export const REGISTER_REQUEST = 'REGISTER_REQUEST';
export const REGISTER_SUCCESS = 'REGISTER_SUCCESS';
export const REGISTER_FAILURE = 'REGISTER_FAILURE';
export const CLEAR_REGISTER_ERROR = 'CLEAR_REGISTER_ERROR';

// Login Actions
export const loginRequest = (credentials) => ({
  type: LOGIN_REQUEST,
  payload: credentials
});

export const loginSuccess = (userData) => ({
  type: LOGIN_SUCCESS,
  payload: userData
});

export const loginFailure = (error) => ({
  type: LOGIN_FAILURE,
  payload: error
});

export const logout = () => ({
  type: LOGOUT
});

export const clearAuthError = () => ({
  type: CLEAR_AUTH_ERROR
});

// Register Actions
export const registerUser = (userData) => ({
  type: REGISTER_REQUEST,
  payload: userData
});

export const registerSuccess = (message) => ({
  type: REGISTER_SUCCESS,
  payload: message
});

export const registerFailure = (error) => ({
  type: REGISTER_FAILURE,
  payload: error
});

export const clearRegisterError = () => ({
  type: CLEAR_REGISTER_ERROR
});
