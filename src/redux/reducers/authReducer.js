import {
  LOGIN_REQUEST,
  LOGIN_SUCCESS,
  LOGIN_FAILURE,
  LOGOUT,
  CLEAR_AUTH_ERROR,
  REGISTER_REQUEST,
  REGISTER_SUCCESS,
  REGISTER_FAILURE,
  CLEAR_REGISTER_ERROR
} from '../actions/authActions';

const initialState = {
  // Login state
  user: null,
  token: null,
  isAuthenticated: false,
  loading: false,
  error: null,
  
  // Register state
  register: {
    loading: false,
    error: null,
    success: false,
    message: null
  }
};

const authReducer = (state = initialState, action) => {
  switch (action.type) {
    // Login cases
    case LOGIN_REQUEST:
      return {
        ...state,
        loading: true,
        error: null
      };
      
    case LOGIN_SUCCESS:
      return {
        ...state,
        loading: false,
        isAuthenticated: true,
        user: action.payload,
        token: action.payload.token,
        error: null
      };
      
    case LOGIN_FAILURE:
      return {
        ...state,
        loading: false,
        isAuthenticated: false,
        user: null,
        token: null,
        error: action.payload
      };
      
    case LOGOUT:
      return {
        ...state,
        isAuthenticated: false,
        user: null,
        token: null,
        error: null
      };
      
    case CLEAR_AUTH_ERROR:
      return {
        ...state,
        error: null
      };
      
    // Register cases
    case REGISTER_REQUEST:
      return {
        ...state,
        register: {
          ...state.register,
          loading: true,
          error: null,
          success: false,
          message: null
        }
      };
      
    case REGISTER_SUCCESS:
      return {
        ...state,
        register: {
          ...state.register,
          loading: false,
          success: true,
          message: action.payload,
          error: null
        }
      };
      
    case REGISTER_FAILURE:
      return {
        ...state,
        register: {
          ...state.register,
          loading: false,
          success: false,
          error: action.payload,
          message: null
        }
      };
      
    case CLEAR_REGISTER_ERROR:
      return {
        ...state,
        register: {
          ...state.register,
          error: null
        }
      };
      
    default:
      return state;
  }
};

export default authReducer;
