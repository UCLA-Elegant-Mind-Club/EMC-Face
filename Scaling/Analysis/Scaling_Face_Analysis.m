%% Data Analysis Script for Eye Tracking Protocol Data
% #sponsored by Mark and Daniel (special thanks to Caominh and Brian)
close all; clear all; clc;

%% Declaring Variables
% monitor dimensions
width_px = 4096;
height_px = 2160;

width_cm = 121;
height_cm = 68;

%participant distance
dist_cm = 121;

% face height in degrees
face = 8;

% axis label step size for x-y plot
step = face/4;

% overlay image
[file,path] = uigetfile('*.png');
img = imread([path,file]);

%% Conversion Factors
x_deg2px = dist_cm*tand(1) * width_px / width_cm;
y_deg2px = dist_cm*tand(1) * height_px / height_cm;

%% Loading Data from Files
[EyeDataName, EyeDataPath] = uigetfile('*.txt'); %opens selection dialog box to select specific eye data
fullEyeDataFilename = fullfile(EyeDataPath, EyeDataName); %concatenates path and filename
rawEyeTrackingData = readtable(fullEyeDataFilename); %FIX?: Imports everything, TobiiState Present/NotPresent imports as NaN

rawEyeTrackingData = rawEyeTrackingData(:,2:end); % Removes first column
rawEyeTrackingData(any(ismissing(rawEyeTrackingData), 2), :) = []; % Removes NaN entries
rawEyeTrackingData = table2array(rawEyeTrackingData); %Convert to array

[RTDataName, RTDataPath] = uigetfile('*.csv'); %opens selection dialog box to select specific trial data
fullRTDataFilename = fullfile(RTDataPath, RTDataName); %concatenates path and filename
RTData = readtable(fullRTDataFilename); %reads all protocol data to table
RTData = table2array(RTData); %Convert to array

%% Processing
breakpoints = RTData(:,5); %finds specific timestamps where face appears (CPU uptime)
maxReadingTime = 5*max(RTData(:,3)); %in milliseconds
rawEyeTracking_Time = rawEyeTrackingData(:,1);
rawEyeTracking_X = rawEyeTrackingData(:,2);
rawEyeTracking_Y = rawEyeTrackingData(:,3);

A = repmat(rawEyeTracking_Time, [1, height(breakpoints)]);
[minValue, closestIndex] = min(abs(A-breakpoints'));
closestValue = rawEyeTracking_X(closestIndex); % Not needed, just interesting, cool to see that theyre all ~2000 px (middle of screen)!

EyeTracking_X = []; % 1000 for max 10 second timeout in protocol
EyeTracking_Y = [];
EyeTracking_Time = [];

%% Plotting
figure(2)
imshow(img);
color = ["#0072BD" "#D95319" "#EDB120" "#7E2F8E" "#77AC30" "#4DBEEE" "#A2142F"];

for i = 1:length(closestIndex)
    EyeTracking_X(:,i) = rawEyeTracking_X(closestIndex(i):closestIndex(i)+90/1000*maxReadingTime);
    EyeTracking_Y(:,i) = rawEyeTracking_Y(closestIndex(i):closestIndex(i)+90/1000*maxReadingTime);
    EyeTracking_Time(:,i) = rawEyeTracking_Time(closestIndex(i):closestIndex(i)+90/1000*maxReadingTime) - rawEyeTracking_Time(closestIndex(i));
    
    B = EyeTracking_Time;
    [minValue2, closestIndex2] = min(abs(B-maxReadingTime'));
    
    hold off
%figure 1: X-time
    figure(1)
    hold on
    plot(EyeTracking_Time(:,i), EyeTracking_X(:,i),'Color',color(i))
    title("X-Time Plot")
    xlabel("Time (s)")
    ylabel("Horizontal Distance (degrees)")
    xlim([0,maxReadingTime]) % Time, in s
    ylim([((width_px/2) - (30*x_deg2px)), ((width_px/2) + (30*x_deg2px))])
    xticks([0,1000,2000,3000,4000,5000,6000,7000,8000,9000,10000])
    xticklabels([0,1,2,3,4,5,6,7,8,9,10])
    yticks([((width_px/2) - (30*x_deg2px)),((width_px/2) - (20*x_deg2px)),((width_px/2) - (10*x_deg2px)), ...
        (width_px/2), ...
        ((width_px/2) + (10*x_deg2px)),((width_px/2) + (20*x_deg2px)),((width_px/2) + (30*x_deg2px))])
    yticklabels([-30,-20,-10,0,10,20,30])
    hold off
%figure 2: X-Y
    figure(2)
    hold on
%comment out EITHER plot(...) or scatter(...) to include lines or not between points
    %plot(EyeTracking_X(1:max(closestIndex2),i), EyeTracking_Y(1:max(closestIndex2),i), "LineWidth", 2, "Marker", '.', "MarkerSize", 20,'Color',color)
    scatter(EyeTracking_X(1:max(closestIndex2),i), EyeTracking_Y(1:max(closestIndex2),i), 30, "Marker", '.', 'Color',color(i))
    title("X-Y Plot: "+ face + " degrees")
    set(gca, 'YDir','reverse')
    xlabel("Horizontal Eccentricity (degrees)")
    ylabel("Vertical Eccentricity (degrees)")
    axis([((width_px/2) - ((face/2)*x_deg2px)) ((width_px/2) + ((face/2)*x_deg2px)) ...
        ((height_px/2) - ((face/2)*y_deg2px)) ((height_px/2) + ((face/2)*y_deg2px))]) % For X_Y Plot
    xticks([((width_px/2) - ((face/2)*x_deg2px)): (step*x_deg2px): ((width_px/2) + ((face/2)*x_deg2px))])
    xticklabels([-(face/2): step: (face/2)])
    yticks([((height_px/2) - ((face/2)*y_deg2px)): (step*y_deg2px): ((height_px/2) + ((face/2)*y_deg2px))])
    yticklabels([-(face/2): step: (face/2)])
    axis on
    set(gca, 'color', 'none', 'box','off');
    hold off
%figure 3: Y-time
    figure(3)
    hold on
    plot(EyeTracking_Time(:,i), EyeTracking_Y(:,i),'Color',color(i))
    set(gca, 'YDir','reverse')
    title("Y-Time Plot")
    xlabel("Time (s)")
    ylabel("Vertical Distance (degrees)")
    xlim([0,maxReadingTime]) % Time, in s
    ylim([((height_px/2) - (20*y_deg2px)), ((height_px/2) + (20*y_deg2px))])
    xticks([0, 1000, 2000, 3000, 4000, 5000,6000,7000,8000,9000,10000])
    xticklabels([0, 1, 2, 3, 4, 5,6,7,8,9,10])
    yticks([((height_px/2) - (20*y_deg2px)), ((height_px/2) - (10*y_deg2px)), ...
        (height_px/2), ...
        ((height_px/2) + (10*y_deg2px)), ((height_px/2) + (20*y_deg2px))])
    yticklabels([-20,-10,0,10,20])
end