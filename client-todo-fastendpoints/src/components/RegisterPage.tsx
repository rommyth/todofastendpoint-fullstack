import { useAuth } from '../store/authStore';
import { useNavigate, Link } from 'react-router-dom';
import { useEffect, type FormEvent } from 'react';

export const RegisterPage = () => {
  const { state, register, isAuthenticated } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (isAuthenticated) navigate('/', { replace: true });
  }, [isAuthenticated, navigate]);

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const form = e.target as HTMLFormElement;
    const name = (form.elements.namedItem('name') as HTMLInputElement).value;
    const email = (form.elements.namedItem('email') as HTMLInputElement).value;
    const password = (form.elements.namedItem('password') as HTMLInputElement).value;
    await register({ name, email, password });
    if (!state.error) navigate('/login', { replace: true });
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-[#0f0f13] via-[#16171d] to-[#1a1b23] flex items-center justify-center p-8">
      <div className="bg-[#1f2028]/80 backdrop-blur-sm rounded-2xl border border-[#2e303a] shadow-xl p-8 w-full max-w-md">
        <div className="text-center mb-8">
          <h1 className="text-4xl font-bold bg-gradient-to-r from-[#c084fc] to-[#e879f9] bg-clip-text text-transparent">
            Create Account
          </h1>
          <p className="text-gray-400 mt-2">Register to start managing your todos</p>
        </div>

        {state.error && (
          <div className="bg-red-500/10 border border-red-500/30 text-red-400 px-4 py-3 rounded-lg mb-6">
            {state.error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-300 mb-2">Name</label>
            <input
              type="text"
              name="name"
              className="w-full px-4 py-3 bg-[#16171d] border border-[#2e303a] rounded-lg focus:outline-none focus:border-[#c084fc] focus:ring-1 focus:ring-[#c084fc] text-white placeholder-gray-500 transition-all"
              placeholder="Enter your name"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-300 mb-2">Email</label>
            <input
              type="email"
              name="email"
              className="w-full px-4 py-3 bg-[#16171d] border border-[#2e303a] rounded-lg focus:outline-none focus:border-[#c084fc] focus:ring-1 focus:ring-[#c084fc] text-white placeholder-gray-500 transition-all"
              placeholder="Enter your email"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-300 mb-2">Password</label>
            <input
              type="password"
              name="password"
              className="w-full px-4 py-3 bg-[#16171d] border border-[#2e303a] rounded-lg focus:outline-none focus:border-[#c084fc] focus:ring-1 focus:ring-[#c084fc] text-white placeholder-gray-500 transition-all"
              placeholder="Create a password"
              required
            />
          </div>
          <button
            type="submit"
            disabled={state.loading}
            className="w-full py-3 bg-gradient-to-r from-[#c084fc] to-[#e879f9] text-white font-medium rounded-lg hover:scale-[1.02] transition-transform duration-200 shadow-lg hover:shadow-[#c084fc]/20 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {state.loading ? 'Creating account...' : 'Create Account'}
          </button>
        </form>

        <p className="text-center mt-6 text-gray-400">
          Already have an account?{' '}
          <Link to="/login" className="text-[#c084fc] hover:text-[#e879f9] transition-colors">
            Sign In
          </Link>
        </p>
      </div>
    </div>
  );
};
