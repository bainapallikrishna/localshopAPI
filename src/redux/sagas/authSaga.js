import { call, put, takeEvery, takeLatest } from 'redux-saga/effects';
import { 
  LOGIN_REQUEST, 
  LOGIN_SUCCESS, 
  LOGIN_FAILURE,
  REGISTER_REQUEST,
  REGISTER_SUCCESS,
  REGISTER_FAILURE
} from '../actions/authActions';

// API base URL - adjust this to match your backend URL
const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:7000/api';

// API call functions
const apiCall = async (url, options = {}) => {
  const response = await fetch(url, {
    headers: {
      'Content-Type': 'application/json',
      ...options.headers,
    },
    ...options,
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({ message: 'Network error' }));
    throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
  }

  return response.json();
};

// Login API call
const loginApi = (credentials) => {
  return apiCall(`${API_BASE_URL}/auth/login`, {
    method: 'POST',
    body: JSON.stringify(credentials),
  });
};

// Register API call
const registerApi = (userData) => {
  return apiCall(`${API_BASE_URL}/auth/register`, {
    method: 'POST',
    body: JSON.stringify(userData),
  });
};

// Login Saga
function* loginSaga(action) {
  try {
    const response = yield call(loginApi, action.payload);
    
    // Store token in localStorage
    localStorage.setItem('token', response.token);
    localStorage.setItem('user', JSON.stringify({
      username: response.username,
      email: response.email,
      roles: response.roles,
      expiresAt: response.expiresAt
    }));
    
    yield put({
      type: LOGIN_SUCCESS,
      payload: response
    });
  } catch (error) {
    yield put({
      type: LOGIN_FAILURE,
      payload: error.message
    });
  }
}

// Register Saga
function* registerSaga(action) {
  try {
    const response = yield call(registerApi, action.payload);
    
    yield put({
      type: REGISTER_SUCCESS,
      payload: response.message || 'Registration successful'
    });
  } catch (error) {
    yield put({
      type: REGISTER_FAILURE,
      payload: error.message
    });
  }
}

// Root auth saga
export function* authSaga() {
  yield takeLatest(LOGIN_REQUEST, loginSaga);
  yield takeLatest(REGISTER_REQUEST, registerSaga);
}
