const ContactPage = () => {
  return (
    <div className="flex flex-col items-center justify-center min-h-[60vh] py-8 bg-gray-50">
      <div className="bg-white rounded-xl shadow-lg p-8 max-w-md w-full">
        <h1 className="text-3xl font-bold text-center mb-6 text-indigo-700">Kontakt</h1>
        <div className="space-y-4 text-center">
          <div>
            <span className="block text-sm font-semibold text-gray-500 mb-1">Adres e-mail:</span>
            <a
              href="mailto:SonicBridge@gmail.com"
              className="text-lg text-indigo-600 hover:underline font-medium"
            >
              SonicBridge@gmail.com
            </a>
          </div>
          <div>
            <span className="block text-sm font-semibold text-gray-500 mb-1">Numer telefonu:</span>
            <a
              href="tel:111-111-111"
              className="text-lg text-indigo-600 hover:underline font-medium"
            >
              111-111-111
            </a>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ContactPage;