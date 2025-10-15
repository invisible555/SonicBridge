import { useSelector } from "react-redux";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Activity, Settings, User } from "lucide-react";
import { Button } from "@/components/ui/button";

const ProfilePage = () => {
  const user = useSelector((state: any) => state.auth.user);
  const role = useSelector((state: any) => state.auth.role);
  const userStats = {
    files: 42,
    transcriptions: 17,
    translations: 28,
    tts: 15,
  };

  return (
    <div className="flex flex-col items-center w-full min-h-screen bg-gradient-to-b from-gray-50 to-gray-100 py-10 px-4">
      {/* === Nagłówek profilu === */}
      <Card className="w-full max-w-4xl shadow-md">
        <CardHeader className="flex flex-col items-center gap-4 py-8">
          <Avatar className="h-24 w-24">
            <AvatarImage src={user?.avatarUrl || "/default-avatar.png"} />
            <AvatarFallback>
              {user?.username?.toUpperCase() || "U"}
              
            </AvatarFallback>
          </Avatar>

          <div className="text-center">
            <CardTitle className="text-2xl font-semibold">
              {user || "Nieznany użytkownik"}
            </CardTitle>
          </div>

          <div className="flex items-center gap-2 mt-2">
            <Badge variant="outline" className="text-sm px-3 py-1">
              {role || "Użytkownik"}
            </Badge>
            <Badge className="bg-green-500 hover:bg-green-600 text-white text-sm px-3 py-1">
              Aktywny
            </Badge>
          </div>

          <div className="mt-4 flex gap-3">
            <Button variant="default">Zarządzaj kontem</Button>
        
          </div>
        </CardHeader>
      </Card>

      {/* === Sekcje szczegółowe === */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6 w-full max-w-4xl mt-10">
        {/* 🔹 Statystyki */}
        <Card className="shadow-sm border border-gray-200">
          <CardHeader className="flex items-center gap-2">
            <Activity className="h-5 w-5 text-blue-600" />
            <CardTitle>Twoja aktywność</CardTitle>
          </CardHeader>
          <CardContent className="grid grid-cols-2 gap-4 mt-4 text-center">
            <div>
              <p className="text-2xl font-semibold">{userStats.files}</p>
              <p className="text-gray-500 text-sm">Pliki</p>
            </div>
            <div>
              <p className="text-2xl font-semibold">{userStats.transcriptions}</p>
              <p className="text-gray-500 text-sm">Transkrypcje</p>
            </div>
            <div>
              <p className="text-2xl font-semibold">{userStats.translations}</p>
              <p className="text-gray-500 text-sm">Tłumaczenia</p>
            </div>
            <div>
              <p className="text-2xl font-semibold">{userStats.tts}</p>
              <p className="text-gray-500 text-sm">Wygenerowane głosy</p>
            </div>
          </CardContent>
        </Card>

        {/* 🔹 Ustawienia użytkownika */}
        <Card className="shadow-sm border border-gray-200">
          <CardHeader className="flex items-center gap-2">
            <Settings className="h-5 w-5 text-purple-600" />
            <CardTitle>Preferencje użytkownika</CardTitle>
          </CardHeader>
          <CardContent className="space-y-3 text-sm text-gray-700 mt-4">
            <div className="flex justify-between">
              <span>Język interfejsu:</span> <span>Polski</span>
            </div>
            <div className="flex justify-between">
              <span>Tryb aplikacji:</span> <span>Jasny</span>
            </div>
            <div className="flex justify-between">
              <span>Powiadomienia:</span> <span>Włączone</span>
            </div>
            <div className="flex justify-between">
              <span>Auto-tłumaczenie:</span> <span>Tak</span>
            </div>
          </CardContent>
        </Card>

        {/* 🔹 Informacje o koncie */}
        <Card className="md:col-span-2 shadow-sm border border-gray-200">
          <CardHeader className="flex items-center gap-2">
            <User className="h-5 w-5 text-gray-700" />
            <CardTitle>Informacje o koncie</CardTitle>
          </CardHeader>
          <CardContent className="text-sm text-gray-700 space-y-2 mt-4">
            <div className="flex justify-between">
              <span>ID użytkownika:</span> <span>{user?.id || "—"}</span>
            </div>
            <div className="flex justify-between">
              <span>Data utworzenia konta:</span>{" "}
              <span>{user?.createdAt || "brak danych"}</span>
            </div>
            <div className="flex justify-between">
              <span>Ostatnie logowanie:</span>{" "}
              <span>{user?.lastLogin || "brak danych"}</span>
            </div>
            <div className="flex justify-between">
              <span>Wersja aplikacji:</span> <span>v1.0.0</span>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
};

export default ProfilePage;
