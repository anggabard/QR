# Step 1: Use the official .NET SDK image for Windows
FROM mcr.microsoft.com/dotnet/sdk:8.0-windows AS build

# Set the working directory inside the container
WORKDIR /app

# Clone the GitHub repository
RUN apt-get update && apt-get install -y git
RUN git clone https://github.com/anggabard/QR.git

# Change to the cloned repository directory
WORKDIR /app/QR

# Restore dependencies
RUN dotnet restore

# Build the function app
RUN dotnet build -c Release --no-restore

# Step 2: Publish the function
RUN dotnet publish -c Release -o /app/publish --no-build

# Step 3: Use the official Azure Functions runtime image for Windows
FROM mcr.microsoft.com/azure-functions/dotnet:4-windows AS base

# Set the working directory inside the container
WORKDIR /home/site/wwwroot

# Copy the published files from the build stage to the runtime stage
COPY --from=build /app/publish /home/site/wwwroot

# Set the entry point to run the function app
ENTRYPOINT ["dotnet", "QR-Generator.exe"]