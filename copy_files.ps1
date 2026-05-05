$src = "e:\New folder\DriveShare\DriveShare.API"
$user = "e:\New folder\DriveShare\DriveShare.UserService"
$rental = "e:\New folder\DriveShare\DriveShare.RentalService"
$notify = "e:\New folder\DriveShare\DriveShare.NotificationService"
$shared = "e:\New folder\DriveShare\DriveShare.Shared"

# Ensure destination folders exist before copying
# Shared
New-Item -ItemType Directory -Force -Path "$shared\Middleware" | Out-Null
New-Item -ItemType Directory -Force -Path "$shared\Helpers" | Out-Null
New-Item -ItemType Directory -Force -Path "$shared\Models\Enums" | Out-Null

# Copy to Shared
Copy-Item "$src\Middleware\*" -Destination "$shared\Middleware" -Recurse -Force -ErrorAction SilentlyContinue
Copy-Item "$src\Helpers\*" -Destination "$shared\Helpers" -Recurse -Force -ErrorAction SilentlyContinue
Copy-Item "$src\Models\Enums\*" -Destination "$shared\Models\Enums" -Recurse -Force -ErrorAction SilentlyContinue

# UserService
New-Item -ItemType Directory -Force -Path "$user\Controllers" | Out-Null
New-Item -ItemType Directory -Force -Path "$user\Models" | Out-Null
New-Item -ItemType Directory -Force -Path "$user\Services\Interfaces" | Out-Null
New-Item -ItemType Directory -Force -Path "$user\DTOs" | Out-Null
New-Item -ItemType Directory -Force -Path "$user\Data" | Out-Null

Copy-Item "$src\Controllers\AuthController.cs" -Destination "$user\Controllers" -Force -ErrorAction SilentlyContinue
Copy-Item "$src\Controllers\UsersController.cs" -Destination "$user\Controllers" -Force -ErrorAction SilentlyContinue
Copy-Item "$src\Controllers\AdminController.cs" -Destination "$user\Controllers" -Force -ErrorAction SilentlyContinue

Copy-Item "$src\Models\User.cs" -Destination "$user\Models" -Force -ErrorAction SilentlyContinue
Copy-Item "$src\Models\Role.cs" -Destination "$user\Models" -Force -ErrorAction SilentlyContinue
Copy-Item "$src\Models\License.cs" -Destination "$user\Models" -Force -ErrorAction SilentlyContinue

Copy-Item "$src\Services\AuthService.cs" -Destination "$user\Services" -Force -ErrorAction SilentlyContinue
Copy-Item "$src\Services\AdminAndLicenseService.cs" -Destination "$user\Services" -Force -ErrorAction SilentlyContinue
Copy-Item "$src\Services\AdminStatsService.cs" -Destination "$user\Services" -Force -ErrorAction SilentlyContinue

Copy-Item "$src\Services\Interfaces\IAuthService.cs" -Destination "$user\Services\Interfaces" -Force -ErrorAction SilentlyContinue
Copy-Item "$src\Services\Interfaces\IAdminService.cs" -Destination "$user\Services\Interfaces" -Force -ErrorAction SilentlyContinue
Copy-Item "$src\Services\Interfaces\IAdminStatsService.cs" -Destination "$user\Services\Interfaces" -Force -ErrorAction SilentlyContinue

Copy-Item "$src\DTOs\Auth*" -Destination "$user\DTOs" -Recurse -Force -ErrorAction SilentlyContinue
Copy-Item "$src\DTOs\User*" -Destination "$user\DTOs" -Recurse -Force -ErrorAction SilentlyContinue
Copy-Item "$src\DTOs\Admin*" -Destination "$user\DTOs" -Recurse -Force -ErrorAction SilentlyContinue

Copy-Item "$src\Data\ApplicationDbContext.cs" -Destination "$user\Data" -Force -ErrorAction SilentlyContinue

# RentalService
New-Item -ItemType Directory -Force -Path "$rental\Controllers" | Out-Null
New-Item -ItemType Directory -Force -Path "$rental\Models" | Out-Null
New-Item -ItemType Directory -Force -Path "$rental\Services\Interfaces" | Out-Null
New-Item -ItemType Directory -Force -Path "$rental\DTOs" | Out-Null
New-Item -ItemType Directory -Force -Path "$rental\Data" | Out-Null
New-Item -ItemType Directory -Force -Path "$rental\wwwroot" | Out-Null

Copy-Item "$src\Controllers\CarsController.cs" -Destination "$rental\Controllers" -Force -ErrorAction SilentlyContinue
Copy-Item "$src\Controllers\BookingsController.cs" -Destination "$rental\Controllers" -Force -ErrorAction SilentlyContinue
Copy-Item "$src\Controllers\ReviewsController.cs" -Destination "$rental\Controllers" -Force -ErrorAction SilentlyContinue

Copy-Item "$src\Models\Car.cs" -Destination "$rental\Models" -Force -ErrorAction SilentlyContinue
Copy-Item "$src\Models\Booking.cs" -Destination "$rental\Models" -Force -ErrorAction SilentlyContinue
Copy-Item "$src\Models\Review.cs" -Destination "$rental\Models" -Force -ErrorAction SilentlyContinue

Copy-Item "$src\Services\CarService.cs" -Destination "$rental\Services" -Force -ErrorAction SilentlyContinue
Copy-Item "$src\Services\BookingService.cs" -Destination "$rental\Services" -Force -ErrorAction SilentlyContinue
Copy-Item "$src\Services\RatingService.cs" -Destination "$rental\Services" -Force -ErrorAction SilentlyContinue

Copy-Item "$src\Services\Interfaces\ICarService.cs" -Destination "$rental\Services\Interfaces" -Force -ErrorAction SilentlyContinue
Copy-Item "$src\Services\Interfaces\IBookingService.cs" -Destination "$rental\Services\Interfaces" -Force -ErrorAction SilentlyContinue
Copy-Item "$src\Services\Interfaces\IRatingService.cs" -Destination "$rental\Services\Interfaces" -Force -ErrorAction SilentlyContinue

Copy-Item "$src\DTOs\Car*" -Destination "$rental\DTOs" -Recurse -Force -ErrorAction SilentlyContinue
Copy-Item "$src\DTOs\Booking*" -Destination "$rental\DTOs" -Recurse -Force -ErrorAction SilentlyContinue
Copy-Item "$src\DTOs\Review*" -Destination "$rental\DTOs" -Recurse -Force -ErrorAction SilentlyContinue
Copy-Item "$src\DTOs\Shared*" -Destination "$rental\DTOs" -Recurse -Force -ErrorAction SilentlyContinue

Copy-Item "$src\Data\ApplicationDbContext.cs" -Destination "$rental\Data\RentalDbContext.cs" -Force -ErrorAction SilentlyContinue
Copy-Item "$src\wwwroot\*" -Destination "$rental\wwwroot" -Recurse -Force -ErrorAction SilentlyContinue

# NotificationService
New-Item -ItemType Directory -Force -Path "$notify\Hubs" | Out-Null
New-Item -ItemType Directory -Force -Path "$notify\Services\Interfaces" | Out-Null

Copy-Item "$src\Hubs\*" -Destination "$notify\Hubs" -Recurse -Force -ErrorAction SilentlyContinue
Copy-Item "$src\Services\NotificationService.cs" -Destination "$notify\Services" -Force -ErrorAction SilentlyContinue
Copy-Item "$src\Services\Interfaces\INotificationService.cs" -Destination "$notify\Services\Interfaces" -Force -ErrorAction SilentlyContinue
